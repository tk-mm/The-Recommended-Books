using CoreTweet;
using NMeCab;
using Sgml;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace WebInfo
{
    public partial class Form1 : Form
    {
        //ネガポジ検出されたワードを格納するリスト
        private List<string> npStr;

        //Twitterアカウントのトークン
        private CoreTweet.Tokens tokens = CoreTweet.Tokens.Create(
                "CONSUMER_KEY"
                , "CONSUMER_SECRET"
                , "ACCESS_TOKEN"
                , "ACCESS_TOKEN_SECRET");

        //楽天ブックスで検索したタイトルのリスト
        private List<string> title, publisherName, largeImageUrl;

        //レスポンスのXMLを格納する変数
        private XmlDocument resultXml;

        //WEBAPIまたは電大メディアセンターのレスポンスに関する変数
        private WebRequest WebReq;

        //リクエストを送るURLのための変数
        private string booksURL, format, hits, genreURL;

        //楽天ブックスのジャンルIDを格納するリスト
        private List<string> id;

        //negative時の再検索キーワードのための変数とインデックス
        private List<string> reqGenreNameList;

        //negative時の再検索ジャンルのための変数とインデックス
        private List<string> reqGenreIdList;

        //検索に用いるキーワードのリストの番号を保持する変数
        private int keyword_index;

        public Form1()
        {
            InitializeComponent();

            //リストボックスに水平スクロールバーを表示
            AnalysisListBox.HorizontalScrollbar = true;
            SearchBooksListBox.HorizontalScrollbar = true;
            MentionsListBox.HorizontalScrollbar = true;
            ScoreOfNegaPosiListBox.HorizontalScrollbar = true;
            DisplayResultListBox.HorizontalScrollbar = true;
        }

        //推薦図書Twitterアカウントのメンションタイムラインを表示
        private void GetMentionsTimeLine_Click(object sender, EventArgs e)
        {
            MentionsListBox.Items.Clear();
            MentionsTimelineAsync();
        }

        //NMeCabによるツイートの形態素解析
        private void TweetAnalysis_Click(object sender, EventArgs e)
        {
            AnalysisListBox.Items.Clear();
            ScoreOfNegaPosiListBox.Items.Clear();
            SearchBooksListBox.Items.Clear();
            DisplayResultListBox.Items.Clear();
            pictureBox1.Image = null;
            if (sentence != null)
            {
                for (int i = 0; i < sentence.Count; i++)
                {
                    if (MentionsListBox.GetSelected(i))
                    {
                        morphological_analysis();
                        NegaposiDB_connection();
                        NegaposiImage();
                    }
                }
            }
        }

        //ネガポジ値の判定をして、そのキーワードを除外するか判定する分岐処理
        private void SearchOfRecommendedBooks_Click(object sender, EventArgs e)
        {
            SearchBooksListBox.Items.Clear();
            DisplayResultListBox.Items.Clear();
            pictureBox1.Image = null;
            if (problem == true)
            {
                if (negaposi_ave >= 0)
                {
                    //楽天ブックスでキーワード検索
                    RakutenBooks_search();
                    PositiveRakutenBooksGenre_search();
                    PositiveRecommendedBooks_search();
                }
                else
                {
                    RakutenBooks_search();
                    NegativeRakutenBooksGenre_search();
                    NegativeRecommendedBooks_search();
                }
                BooksImageSave();
            }
        }

        //SearchBooksListBoxに推薦された図書を選択すると、大学図書館にその本があるか内部的に検索する
        private void SearchBooksListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayResultListBox.Items.Clear();
            pictureBox1.Image = null;
            MediaCenter_RakutenBooks_match();
            ResultView();
        }

        //推薦結果をリプライでツイート
        private void ResultOfTweets_Click(object sender, EventArgs e)
        {
            DisplayResultListBox.Items.Clear();
            if (recommendedResult != null)
            {
                ReplyRecommendedBooks();
            }
        }

        //リプライツイートを格納するリスト
        private List<string> sentence;

        //NMeCabによるツイートの形態素解析
        private void morphological_analysis()
        {
            List<string> surfaceList = new List<string>();
            List<string> featurList = new List<string>();

            // NMeCab はクラス名が変更されている
            MeCabTagger t = MeCabTagger.Create();

            // 形態素を一つずつたどっていく
            MeCabNode node = t.ParseToNode(sentence[MentionsListBox.SelectedIndex]);

            int count = 0;
            while (node != null)
            {
                if (node.Surface == sentence[MentionsListBox.SelectedIndex])
                {
                    node = node.Next;
                    continue;
                }
                if (node.Surface == "#" || node.Surface == "+" || node.Surface == "言語")
                {
                    surfaceList[count - 1] += node.Surface;
                    featurList[count - 1] += node.Feature;
                    node = node.Next;
                    continue;
                }

                surfaceList.Add(node.Surface);
                featurList.Add(node.Feature);
                node = node.Next;
                count++;
            }
            for (int i = 0; i < surfaceList.Count; i++)
            {
                AnalysisListBox.Items.Add(surfaceList[i] + "\t" + featurList[i]);
            }
            npStr = new List<string>();
            foreach (String surface in surfaceList)
            {
                npStr.Add(surface);
            }
            count = 0;
            foreach (var featur in featurList)
            {
                if (featur == "名詞,一般,*,*,*,*,*")
                {
                    keyword_index = count;
                }
                count++;
            }
            count = 0;
            foreach (var featur in featurList)
            {
                if (featur == "名詞,固有名詞,組織,*,*,*,*")
                {
                    keyword_index = count;
                }
                count++;
            }
            Console.WriteLine("固有名詞のインデックス：" + keyword_index);
        }

        //ネガポジの平均値を保持する変数
        private double negaposi_ave;

        //SearchOfRecommendedBooksボタンを押した時の例外を除去する変数
        private bool problem = false;

        //単語感情極性対応表DBの接続、ツイートのネガポジ値解析
        private void NegaposiDB_connection()
        {
            var fi = new FileInfo("Negaposi.db3");
            if (fi.Exists)
            {
                try
                {
                    var connectionString = new SQLiteConnectionStringBuilder
                    {
                        DataSource = fi.FullName
                    };

                    using (var connection = new SQLiteConnection(connectionString.ToString()))
                    {
                        var context = new DataContext(connection);
                        var negaposi = context.GetTable<Negaposi>();
                        Console.WriteLine("-----------------------------------");
                        Console.WriteLine("----------Database Loaded----------");

                        var q = npStr.Select(x => new Negaposi(x));

                        var s = negaposi
                            .AsEnumerable()
                            .Intersect(q);
                        foreach (var row in s)
                        {
                            ScoreOfNegaPosiListBox.Items.Add(row);
                            Console.WriteLine(row);
                        }
                        negaposi_ave = s.Average(x => x.point) + 0.3;
                        Console.WriteLine(s.Average(x => x.point) + 0.3);
                    }
                    ScoreOfNegaPosiListBox.Items.Add("このツイートのスコア：【" + negaposi_ave + "】");
                    Console.WriteLine("-----------------------------------");
                }
                catch (SQLiteException exc)
                {
                    Console.WriteLine(exc.StackTrace);
                }
                problem = true;
            }
        }

        //ネガポジ値に応じた画像を表示する
        private void NegaposiImage()
        {
            if (negaposi_ave >= 0)
            {
                var image = new Bitmap("happy.png");
                pictureBox2.Image = image;
            }
            else
            {
                var image = new Bitmap("cry.png");
                pictureBox2.Image = image;
            }
        }

        //楽天ブックスの検索
        private void RakutenBooks_search()
        {
            try
            {
                string search = (string)npStr[keyword_index];
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("検索するワード：【" + search + "】");
                Console.WriteLine("-----------------------------------");
                //UTF8でバイト配列にエンコードする
                var postData = HttpUtility.UrlEncode(search, Encoding.UTF8);

                booksURL = "https://app.rakuten.co.jp/services/api/BooksBook/Search/20130522?applicationId=1039656734163992203&title=";
                format = "&format=xml";
                hits = "&hits=10";
                //楽天ブックスのキーワード検索 "C%23%e8%a8%80%e8%aa%9e"
                WebReq = WebRequest.Create(booksURL + postData + hits + format);

                //結果をうけとってDOMオブジェクトにする
                WebResponse webRes = WebReq.GetResponse();
                XmlDocument resultXml = new XmlDocument();

                using (StreamReader reader = new StreamReader(webRes.GetResponseStream()))
                {
                    resultXml.Load(reader);
                }

                //結果XML中のbooksGenreIdタグのリストを取得する
                XmlNodeList idList = resultXml.GetElementsByTagName("booksGenreId");

                //ジャンルIDのリストを生成
                id = new List<string>();
                foreach (XmlNode temp in idList)
                {
                    foreach (XmlNode idNode in temp.ChildNodes)
                    {
                        id.Add(idNode.InnerText);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(@"File Not Found あるいはSgmlReaderDll.dllがありません");
            }
            catch (FileLoadException)
            {
                Console.WriteLine("File Load Error");
            }
            catch (Exception e2)
            {
                Console.WriteLine(e2);
            }
        }

        private void PositiveRakutenBooksGenre_search()
        {
            try
            {
                format = "&format=xml";
                List<string> splitword = new List<string>();
                foreach (string word in id)
                {
                    if (word.Contains('/'))
                    {
                        foreach (var item in word.Split('/'))
                        {
                            splitword.Add(item);
                        }
                    }
                    else
                    {
                        splitword.Add(word);
                    }
                }

                genreURL = "https://app.rakuten.co.jp/services/api/BooksGenre/Search/20121128?applicationId=1039656734163992203&booksGenreId=";

                WebRequest WebReqList;
                WebResponse webResList;
                nameList = new List<string>();
                IdList = new List<string>();
                foreach (var item in splitword)
                {
                    WebReqList = WebRequest.Create(genreURL + item + format);
                    webResList = WebReqList.GetResponse();
                    resultXml = new XmlDocument();
                    using (StreamReader reader = new StreamReader(webResList.GetResponseStream()))
                    {
                        resultXml.Load(reader);
                    }
                    //root
                    XmlNode root = resultXml.DocumentElement;
                    XmlNodeList current = root.SelectNodes("current");

                    //positive
                    foreach (XmlNode temp in current)
                    {
                        nameList.Add(temp.SelectSingleNode("booksGenreName").InnerText);
                        IdList.Add(temp.SelectSingleNode("booksGenreId").InnerText);
                    }
                }

                //ジャンル名をそれぞれ数える
                Dictionary<string, int> genreName = new Dictionary<string, int>();
                int value = 1;
                foreach (var item in nameList)
                {
                    if (genreName.ContainsKey(item))
                    {
                        genreName[item] += value;
                    }
                    else
                    {
                        genreName.Add(item, value);
                    }
                }

                string first_word = "";
                //一番多いジャンルの要素数
                int first = 0;
                foreach (KeyValuePair<string, int> item in genreName)
                {
                    if (item.Key.Contains("その他"))
                    {
                        continue;
                    }
                    if (first < item.Value)
                    {
                        first = item.Value;
                        first_word = item.Key;
                    }
                }
                int index = 0;
                first = 0;
                foreach (var item in nameList)
                {
                    if (item.Contains(first_word))
                    {
                        index = first;
                    }
                    first++;
                }

                //頻出ジャンルを格納
                req_genre = new List<string>();
                if (first_word.Contains("・"))
                {
                    foreach (var item in first_word.Split('・'))
                    {
                        req_genre.Add(item);
                    }
                }
                else
                {
                    req_genre.Add(first_word);
                }

                Console.WriteLine("-----------------------------------");
                foreach (var item in req_genre)
                {
                    Console.WriteLine("選抜ジャンル名：【" + item + "】");
                }
                //頻出ジャンルのジャンルIDを格納する
                posiGerneId = IdList[index];
                Console.WriteLine("選抜ジャンルのID" + posiGerneId);
                Console.WriteLine("-----------------------------------");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(@"File Not Found あるいはSgmlReaderDll.dllがありません");
            }
            catch (FileLoadException)
            {
                Console.WriteLine("File Load Error");
            }
            catch (Exception e2)
            {
                Console.WriteLine(e2);
            }
        }

        //ネガポジ値がポジティブだったとき、そのキーワード上位3件の本をSearchBooksListBoxに表示
        private void PositiveRecommendedBooks_search()
        {
            SearchBooksListBox.Items.Clear();
            try
            {
                string search = (string)npStr[keyword_index];
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("検索するワード：【" + search + "】");
                Console.WriteLine("-----------------------------------");
                //UTF8でバイト配列にエンコードする
                var postData = HttpUtility.UrlEncode(search, Encoding.UTF8);

                //var encsort = HttpUtility.UrlEncode("sales", Encoding.UTF8);
                //新しい順にソート
                var releaseDate = HttpUtility.UrlEncode("-releaseDate", Encoding.UTF8);
                var genre = "&booksGenreId=" + posiGerneId;

                //楽天のApplicationId
                booksURL = "https://app.rakuten.co.jp/services/api/BooksBook/Search/20130522?applicationId=XXXXXXXX&title=";
                format = "&format=xml";
                hits = "&hits=10";
                //楽天ブックスのキーワード検索
                WebReq = WebRequest.Create(booksURL + postData + genre + hits + "&sort=" + releaseDate + format);

                //結果をうけとってDOMオブジェクトにする
                WebResponse webRes = WebReq.GetResponse();
                XmlDocument resultXml = new XmlDocument();

                using (StreamReader reader = new StreamReader(webRes.GetResponseStream()))
                {
                    resultXml.Load(reader);
                }

                //結果XML中のタグのリストを取得する
                XmlNode root = resultXml.DocumentElement;
                XmlNodeList titleList = root.SelectNodes("Items/Item");
                XmlNodeList publisherNameList = root.SelectNodes("Items/Item");
                XmlNodeList mediumImageUrlList = root.SelectNodes("Items/Item");

                //タイトルのリストを生成
                title = new List<string>();
                //出版社のリストを生成
                publisherName = new List<string>();
                //画像URLのリストを生成
                largeImageUrl = new List<string>();

                int seed = Environment.TickCount;
                int choice1 = -1;
                int choice2 = -1;
                int choice3 = -1;
                while (choice3 == -1)
                {
                    Random rand = new Random(seed++);

                    if (choice1 == -1)
                    {
                        choice1 = rand.Next(0, 9);
                        continue;
                    }
                    else if (choice2 == -1)
                    {
                        choice2 = rand.Next(0, 9);
                        if (choice1 == choice2)
                        {
                            choice2 = -1;
                            continue;
                        }
                        continue;
                    }
                    else if (choice3 == -1)
                    {
                        choice3 = rand.Next(0, 9);
                        if (choice1 == choice3 || choice2 == choice3)
                        {
                            choice3 = -1;
                            continue;
                        }
                        continue;
                    }
                }

                List<int> choice = new List<int>();
                choice.Add(choice1);
                choice.Add(choice2);
                choice.Add(choice3);
                choice.Sort();

                foreach (var temp in choice)
                {
                    title.Add(titleList[temp].SelectSingleNode("title").InnerText);
                    SearchBooksListBox.Items.Add("タイトル：" + titleList[temp].SelectSingleNode("title").InnerText);
                    publisherName.Add(publisherNameList[temp].SelectSingleNode("publisherName").InnerText);
                    largeImageUrl.Add(mediumImageUrlList[temp].SelectSingleNode("mediumImageUrl").InnerText);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(@"File Not Found あるいはSgmlReaderDll.dllがありません");
            }
            catch (FileLoadException)
            {
                Console.WriteLine("File Load Error");
            }
            catch (Exception e2)
            {
                Console.WriteLine(e2);
            }
        }

        //電大メディアセンターのHTMLスクレイピングするため
        private static XDocument ParseHtml(TextReader reader)
        {
            using (var sgmlReader = new SgmlReader { DocType = "HTML", CaseFolding = CaseFolding.ToLower })
            {
                sgmlReader.IgnoreDtd = true;
                sgmlReader.InputStream = reader;
                return XDocument.Load(sgmlReader);
            }
        }

        //本のジャンル名とIDを格納するリスト
        private List<string> nameList, IdList;

        //ネガティブ時の再検索で除くキーワードを格納する変数
        private string ng_word;

        //頻出ジャンルを格納するリスト
        private List<string> req_genre;

        //ポジティブなときのジャンルID
        private string posiGerneId;

        //楽天ブックスジャンル検索
        //「Java」で検索した結果の本からジャンル名を取得できる。ジャンルは複数含むこともある。
        private void NegativeRakutenBooksGenre_search()
        {
            try
            {
                format = "&format=xml";
                List<string> splitword = new List<string>();
                foreach (string word in id)
                {
                    if (word.Contains('/'))
                    {
                        foreach (var item in word.Split('/'))
                        {
                            splitword.Add(item);
                        }
                    }
                    else
                    {
                        splitword.Add(word);
                    }
                }

                genreURL = "https://app.rakuten.co.jp/services/api/BooksGenre/Search/20121128?applicationId=XXXXXXXX&booksGenreId=";

                WebRequest WebReqList;
                WebResponse webResList;
                nameList = new List<string>();
                IdList = new List<string>();
                for (int i = 0; i < splitword.Count; i++)
                {
                    WebReqList = WebRequest.Create(genreURL + splitword[i] + format);
                    webResList = WebReqList.GetResponse();
                    resultXml = new XmlDocument();
                    using (StreamReader reader = new StreamReader(webResList.GetResponseStream()))
                    {
                        resultXml.Load(reader);
                    }
                    //root
                    XmlNode root = resultXml.DocumentElement;
                    XmlNodeList parents = root.SelectNodes("parents/parent");

                    //negative
                    foreach (XmlNode item in parents)
                    {
                        nameList.Add(item.SelectSingleNode("booksGenreName").InnerText);
                        IdList.Add(item.SelectSingleNode("booksGenreId").InnerText);
                    }
                }

                //ジャンルIDをそれぞれ数える
                int count = 1;
                Dictionary<string, int> genreId = new Dictionary<string, int>();
                foreach (var item in IdList)
                {
                    if (genreId.ContainsKey(item))
                    {
                        genreId[item] += count;
                    }
                    else
                    {
                        genreId.Add(item, count);
                    }
                }

                //ジャンル名をそれぞれ数える
                Dictionary<string, int> genreName = new Dictionary<string, int>();
                int value = 1;
                foreach (var item in nameList)
                {
                    if (genreName.ContainsKey(item))
                    {
                        genreName[item] += value;
                    }
                    else
                    {
                        genreName.Add(item, value);
                    }
                }

                string first_word = "";

                //一番多いジャンルの要素数
                int first = 0;
                foreach (KeyValuePair<string, int> item in genreName)
                {
                    if (item.Key.Contains("その他"))
                    {
                        continue;
                    }
                    else if (item.Key == npStr[keyword_index])
                    {
                        ng_word = item.Key;
                        continue;
                    }
                    else
                    {
                        ng_word = npStr[keyword_index];
                    }
                    if (first < item.Value)
                    {
                        first = item.Value;
                        first_word = item.Key;
                    }
                }

                int ng_wordIndex = 0;
                foreach (KeyValuePair<string, int> item in genreName)
                {
                    if (item.Key.Contains("その他"))
                    {
                        continue;
                    }
                    else if (ng_wordIndex < item.Value)
                    {
                        ng_wordIndex = item.Value;
                    }
                }

                int second = 0;
                string second_word = "";
                foreach (KeyValuePair<string, int> item in genreName)
                {
                    if (item.Key.Contains("その他"))
                    {
                        continue;
                    }
                    else if (second == first)
                    {
                        continue;
                    }
                    else if (item.Key == npStr[keyword_index])
                    {
                        continue;
                    }
                    else if (item.Key == first_word)
                    {
                        continue;
                    }
                    if (second < item.Value)
                    {
                        second = item.Value;
                        second_word = item.Key;
                    }
                }

                //頻出ジャンル名リストをリストに格納
                reqGenreNameList = new List<string>();
                //1番目の頻出ジャンル名をリストに格納
                reqGenreNameList.Add(first_word);
                //2番目の頻出ジャンル名をリストに格納
                reqGenreNameList.Add(second_word);

                //1番目と2番目の要素数をリストに格納する
                List<int> index = new List<int>();
                count = 1;
                //頻出ジャンルをリストの格納
                req_genre = new List<string>();
                foreach (var item in reqGenreNameList)
                {
                    if (count == 1)
                    {
                        if (item.Contains("・"))
                        {
                            foreach (var item2 in item.Split('・'))
                            {
                                req_genre.Add(item2);
                                index.Add(first);
                            }
                        }
                        else
                        {
                            req_genre.Add(item);
                            index.Add(first);
                        }
                    }
                    if (count == 2)
                    {
                        if (item.Contains("・"))
                        {
                            foreach (var item2 in item.Split('・'))
                            {
                                req_genre.Add(item2);
                                index.Add(second);
                            }
                        }
                        else
                        {
                            req_genre.Add(item);
                            index.Add(second);
                        }
                    }
                    count++;
                }

                Console.WriteLine("-----------------------------------");
                foreach (var item in req_genre)
                {
                    Console.WriteLine("選抜ジャンル：【" + item + "】");
                }

                //選抜ジャンルのジャンルIDを格納
                List<string> req_genreId = new List<string>();
                foreach (var i in index)
                {
                    foreach (KeyValuePair<string, int> item in genreId)
                    {
                        if (i == item.Value)
                        {
                            req_genreId.Add(item.Key);
                        }
                    }
                }
                //選抜ジャンルIDリスト
                reqGenreIdList = new List<string>();
                for (int i = 0; i < index.Count; i++)
                {
                    reqGenreIdList.Add(req_genreId[i]);
                }
                foreach (var item in reqGenreIdList)
                {
                    Console.WriteLine("選抜ID：【" + item + "】");
                }
                Console.WriteLine("NGkeyword【" + ng_word + "】");
                Console.WriteLine("-----------------------------------");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(@"File Not Found あるいはSgmlReaderDll.dllがありません");
            }
            catch (FileLoadException)
            {
                Console.WriteLine("File Load Error");
            }
            catch (Exception e2)
            {
                Console.WriteLine(e2);
            }
        }

        //推薦図書検索をする
        private void NegativeRecommendedBooks_search()
        {
            int count = 0;
            try
            {
                //タイトルのリストを生成
                title = new List<string>();
                //出版社のリストを生成
                publisherName = new List<string>();
                //画像URLのリストを生成
                largeImageUrl = new List<string>();
                foreach (var item in req_genre)
                {
                    XmlDocument doc = new XmlDocument();
                    string search = item;

                    booksURL = "https://app.rakuten.co.jp/services/api/BooksTotal/Search/20130522?applicationId=XXXXXXXX&keyword=";
                    format = "&format=xml";
                    hits = "&hits=10";
                    string sort = "&sort=";
                    //var sales = HttpUtility.UrlEncode("sales", Encoding.UTF8);
                    var releaseDate = HttpUtility.UrlEncode("-releaseDate", Encoding.UTF8);
                    var genre = "&booksGenreId=" + reqGenreIdList[count];

                    var NGKeyword = "&NGKeyword=" + HttpUtility.UrlEncode(ng_word, Encoding.UTF8);

                    //楽天ブックス総合検索のキーワード検索
                    WebReq = WebRequest.Create(booksURL + search + genre + NGKeyword + sort + releaseDate + hits + format);
                    //結果をうけとってDOMオブジェクトにする
                    WebResponse webRes = WebReq.GetResponse();
                    XmlDocument resultXml = new XmlDocument();

                    using (StreamReader reader = new StreamReader(webRes.GetResponseStream()))
                    {
                        resultXml.Load(reader);
                    }

                    XmlNode root = resultXml.DocumentElement;
                    XmlNodeList titleList = root.SelectNodes("Items/Item");
                    XmlNodeList publisherNameList = root.SelectNodes("Items/Item");
                    XmlNodeList mediumImageUrlList = root.SelectNodes("Items/Item");

                    int seed = Environment.TickCount;
                    int choice1 = -1;
                    int choice2 = -1;
                    int choice3 = -1;
                    while (choice3 == -1)
                    {
                        Random rand = new Random(seed++);

                        if (choice1 == -1)
                        {
                            choice1 = rand.Next(0, 9);
                            continue;
                        }
                        else if (choice2 == -1)
                        {
                            choice2 = rand.Next(0, 9);
                            if (choice1 == choice2)
                            {
                                choice2 = -1;
                                continue;
                            }
                            continue;
                        }
                        else if (choice3 == -1)
                        {
                            choice3 = rand.Next(0, 9);
                            if (choice1 == choice3 || choice2 == choice3)
                            {
                                choice3 = -1;
                                continue;
                            }
                            continue;
                        }
                    }

                    List<int> choice = new List<int>();
                    choice.Add(choice1);
                    choice.Add(choice2);
                    choice.Add(choice3);
                    choice.Sort();

                    foreach (var temp in choice)
                    {
                        title.Add(titleList[temp].SelectSingleNode("title").InnerText);
                        SearchBooksListBox.Items.Add("タイトル：" + titleList[temp].SelectSingleNode("title").InnerText);
                        publisherName.Add(publisherNameList[temp].SelectSingleNode("publisherName").InnerText);
                        largeImageUrl.Add(mediumImageUrlList[temp].SelectSingleNode("mediumImageUrl").InnerText);
                    }
                    count++;
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(@"File Not Found あるいはSgmlReaderDll.dllがありません");
            }
            catch (FileLoadException)
            {
                Console.WriteLine("File Load Error");
            }
            catch (Exception e2)
            {
                Console.WriteLine(e2);
            }
        }

        //タイトル検索して見つけた本のデータを格納するリスト
        private List<string> exist, re_seachList;

        //重複を取り除くために用いる変数
        private HashSet<string> setSearch;

        //電大図書館に蔵書があるかマッチしたらその本のタイトルを保持、マッチしなかったらなしという結果を保持する変数
        private string recommendedResult = null;

        //電大図書館に蔵書があるか検索する
        private void MediaCenter_RakutenBooks_match()
        {
            try
            {
                re_seachList = new List<string>();
                exist = new List<string>();
                string re_search = title[SearchBooksListBox.SelectedIndex];

                using (var stream = new WebClient().OpenRead("http://lib.mrcl.dendai.ac.jp/webopac/ctlsrh.do?words=" + re_search + "&listcnt=3"))
                using (var sr = new StreamReader(stream))
                {
                    var xml = ParseHtml(sr);
                    foreach (var item in xml.Descendants("tr"))
                    {
                        foreach (var item1 in item.Descendants("td"))
                        {
                            foreach (var item2 in item1.Descendants("div"))
                            {
                                foreach (var item3 in item2.Descendants("img"))
                                {
                                    exist.Add((string)item3.Value.Trim());
                                }
                            }
                        }
                    }
                    if (!exist[exist.Count - 2].Contains("指定された条件に該当する資料がありませんでした。"))
                    {
                        foreach (var item in xml.Descendants("tr"))
                        {
                            foreach (var item1 in item.Descendants("td"))
                            {
                                foreach (var item2 in item1.Descendants("div"))
                                {
                                    foreach (var item3 in item2.Descendants("strong"))
                                    {
                                        re_seachList.Add(item3.Value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        recommendedResult = exist[exist.Count - 2];
                    }
                }
                List<string> splitword = new List<string>();

                if (re_search.Contains('　'))
                {
                    foreach (var item in re_search.Split('　'))
                    {
                        splitword.Add(item);
                    }
                }
                else
                {
                    splitword.Add(re_search);
                }
                if (re_search.Contains('：'))
                {
                    foreach (var item in re_search.Split('：'))
                    {
                        splitword.Add(item);
                    }
                }
                else
                {
                    splitword.Add(re_search);
                }
                if (re_search.Contains("「"))
                {
                    re_search = re_search.Replace("「", "");
                    re_search = re_search.Replace("」", "");
                    splitword.Add(re_search);
                }

                //重複を消すためハッシュセットを使う
                setSearch = new HashSet<string>();
                foreach (var item in re_seachList)
                {
                    if (item == "・" || item == "1" || item == "2" || item == "3" || item == "4" || item == "5")
                    {
                        continue;
                    }
                    for (int i = 0; i < splitword.Count; i++)
                    {
                        if (item.Contains(splitword[i]))
                        {
                            setSearch.Add(item);
                        }
                    }
                }
                foreach (var item in setSearch)
                {
                    recommendedResult = item;
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(@"File Not Found あるいはSgmlReaderDll.dllがありません");
            }
            catch (FileLoadException)
            {
                Console.WriteLine("File Load Error");
            }
            catch (Exception e2)
            {
                Console.WriteLine(e2);
            }
        }

        //リプライしたユーザのツイートIDを保持するリスト
        private List<long> replyUserID;

        //リプライしたユーザの名前を保持するリスト
        private List<string> replyUserName;

        //メンションタイムラインを表示
        public async void MentionsTimelineAsync()
        {
            int index = 0;
            replyUserID = new List<long>();
            replyUserName = new List<string>();
            sentence = new List<string>();
            foreach (var status in await tokens.Statuses.MentionsTimelineAsync(count => 10))
            {
                var name = status.User.Name;
                //var showedStatus = tokens.Statuses.Show(id => status.Id);
                sentence.Add(status.Text);
                replyUserID.Add(status.Id);
                replyUserName.Add(status.User.ScreenName);

                sentence[index] = sentence[index].Replace("@" + status.InReplyToScreenName + " ", "");
                MentionsListBox.Items.Add(name + "    " + sentence[index]);
                index++;
            }
        }

        //推薦図書をリプライする
        public void ReplyRecommendedBooks()
        {
            if (recommendedResult == "指定された条件に該当する資料がありませんでした。")
            {
                var text1 = "@" + replyUserName[MentionsListBox.SelectedIndex] + " 推薦したい本が図書館にないみたいです。";
                var text2 = publisherName[SearchBooksListBox.SelectedIndex] + "って出版社の";
                var text3 = "「" + title[SearchBooksListBox.SelectedIndex] + "」って本なんですけど・・・";
                var text4 = "もしよかったら大学に購入申請してね！";
                //画像付きリプライをツイートする
                tokens.Statuses.UpdateWithMediaAsync(status => text1 + text2 + text3 + text4,
                    in_reply_to_status_id => replyUserID[MentionsListBox.SelectedIndex],
                    media => new FileInfo("booksImage" + SearchBooksListBox.SelectedIndex + ".jpg"));
                //ツイート結果をGUIでも表示
                var image = new Bitmap("booksImage" + SearchBooksListBox.SelectedIndex + ".jpg");
                pictureBox1.Image = image;
                DisplayResultListBox.Items.Add(" 推薦したい本が図書館にないみたいです。");
                DisplayResultListBox.Items.Add(text2);
                DisplayResultListBox.Items.Add(text3);
                DisplayResultListBox.Items.Add(text4);
            }
            else
            {
                var text1 = "@" + replyUserName[MentionsListBox.SelectedIndex] + " 推薦したい本が図書館にありました！";
                var text2 = " 「" + recommendedResult;
                var text3 = "」なんて本はどうですか？";
                //画像付きリプライをツイートする
                tokens.Statuses.UpdateWithMediaAsync(status => text1 + text2 + text3,
                    in_reply_to_status_id => replyUserID[MentionsListBox.SelectedIndex],
                    media => new FileInfo("booksImage" + SearchBooksListBox.SelectedIndex + ".jpg"));
                //ツイート結果をGUIでも表示
                var image = new Bitmap("booksImage" + SearchBooksListBox.SelectedIndex + ".jpg");
                pictureBox1.Image = image;
                DisplayResultListBox.Items.Add(text2);
                DisplayResultListBox.Items.Add(text3);
            }
        }

        //推薦図書のイメージ画像を保存する
        private void BooksImageSave()
        {
            int index = 0;
            string dir = System.Environment.CurrentDirectory;
            foreach (var item in largeImageUrl)
            {
                WebClient wc = new WebClient(); //System.Net
                Uri u = new Uri(item);

                try
                {
                    wc.DownloadFileAsync(u, "booksImage" + index + ".jpg");
                }
                catch (WebException exc)
                {
                    Console.WriteLine(exc);
                }
                index++;

                wc.Dispose();
            }
        }

        //GUIにツイート内容を表示
        private void ResultView()
        {
            if (recommendedResult == "指定された条件に該当する資料がありませんでした。")
            {
                var text1 = " 推薦したい本が図書館にないみたいです。";
                var text2 = publisherName[SearchBooksListBox.SelectedIndex] + "って出版社の";
                var text3 = "「" + title[SearchBooksListBox.SelectedIndex] + "」";
                var text4 = "って本なんですけど・・・";
                var text5 = "もしよかったら大学に購入申請してください！";
                //ツイート結果をGUIでも表示
                var image = new Bitmap("booksImage" + SearchBooksListBox.SelectedIndex + ".jpg");
                pictureBox1.Image = image;
                DisplayResultListBox.Items.Add(text1);
                DisplayResultListBox.Items.Add(text2);
                DisplayResultListBox.Items.Add(text3);
                DisplayResultListBox.Items.Add(text4);
                DisplayResultListBox.Items.Add(text5);
            }
            else
            {
                var text1 = " 推薦したい本が図書館にありました！";
                var text2 = " 「" + recommendedResult + "」";
                var text3 = "なんて本はどうですか？";
                //ツイート結果をGUIでも表示
                var image = new Bitmap("booksImage" + SearchBooksListBox.SelectedIndex + ".jpg");
                pictureBox1.Image = image;
                DisplayResultListBox.Items.Add(text1);
                DisplayResultListBox.Items.Add(text2);
                DisplayResultListBox.Items.Add(text3);
            }
        }
    }
}
