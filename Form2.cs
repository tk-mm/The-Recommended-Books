using Sgml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WebInfo
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            this.label1.BackColor = Color.Transparent;
            this.label2.BackColor = Color.Transparent;

            ActiveControl = this.textBox1;

            button1.Enabled = false;
        }


        //電大図書館WebページのHTMLスクレイピングするため
        private static XDocument ParseHtml(TextReader reader)
        {
            using (var sgmlReader = new SgmlReader { DocType = "HTML", CaseFolding = CaseFolding.ToLower })
            {
                sgmlReader.IgnoreDtd = true;
                sgmlReader.InputStream = reader;
                return XDocument.Load(sgmlReader);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("学籍番号またはパスワードに入力がされていません。",
                        "ログインエラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
            }
            else
            {
                Login();
            }
        }

        private void Login()
        {
            string id = textBox1.Text;
            string pass = textBox2.Text;
            string access = null;
            try
            {
                using (var stream = new WebClient().OpenRead("https://lib.mrcl.dendai.ac.jp/webopac/askidf.do?display=catsrd&userid=" + id + "&password=" + pass + "&x=36&y=10"))
                using (var sr = new StreamReader(stream))
                {
                    var xml = ParseHtml(sr);
                    foreach (var item in xml.Descendants("tr"))
                    {
                        foreach (var item1 in item.Descendants("td"))
                        {
                            foreach (var item2 in item1.Descendants("br"))
                            {
                                foreach (var item3 in item2.Descendants("font"))
                                {
                                    foreach (var item4 in item3.Descendants("strong"))
                                    {
                                        access = (string)item4;
                                    }
                                }
                            }
                        }
                    }
                }

                if (access != "OP-0404-E&nbsp;")
                {
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("正しい学籍番号またはパスワードを入力してください。",
                        "ログインエラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    textBox2.Clear();
                    ActiveControl = this.textBox2;
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.textBox1.Text) && !string.IsNullOrWhiteSpace(this.textBox2.Text))
            {
                this.button1.Enabled = true;
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                if (textBox1.Text == "" || textBox2.Text == "")
                {
                    MessageBox.Show("学籍番号またはパスワードに入力がされていません。",
                            "ログインエラー",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                }
                else
                {
                    Login();
                }
            }
        }
    }
}
