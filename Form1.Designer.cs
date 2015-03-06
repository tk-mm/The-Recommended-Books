namespace WebInfo
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.AnalysisListBox = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SearchBooksListBox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.GetMentionsTimeLine = new System.Windows.Forms.Button();
            this.MentionsListBox = new System.Windows.Forms.ListBox();
            this.button5 = new System.Windows.Forms.Button();
            this.ScoreOfNegaPosiListBox = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.DisplayResultListBox = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(-7, 259);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(212, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "【Analysis of Tweets】";
            // 
            // AnalysisListBox
            // 
            this.AnalysisListBox.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.AnalysisListBox.FormattingEnabled = true;
            this.AnalysisListBox.ItemHeight = 22;
            this.AnalysisListBox.Location = new System.Drawing.Point(9, 295);
            this.AnalysisListBox.Name = "AnalysisListBox";
            this.AnalysisListBox.Size = new System.Drawing.Size(399, 224);
            this.AnalysisListBox.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button1.Location = new System.Drawing.Point(201, 257);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(207, 31);
            this.button1.TabIndex = 2;
            this.button1.Text = "② Analysis";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.TweetAnalysis_Click);
            // 
            // SearchBooksListBox
            // 
            this.SearchBooksListBox.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SearchBooksListBox.FormattingEnabled = true;
            this.SearchBooksListBox.ItemHeight = 22;
            this.SearchBooksListBox.Location = new System.Drawing.Point(441, 295);
            this.SearchBooksListBox.Name = "SearchBooksListBox";
            this.SearchBooksListBox.Size = new System.Drawing.Size(527, 224);
            this.SearchBooksListBox.TabIndex = 3;
            this.SearchBooksListBox.SelectedIndexChanged += new System.EventHandler(this.SearchBooksListBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(422, 259);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(331, 26);
            this.label2.TabIndex = 4;
            this.label2.Text = "【Search of recommended books】";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(422, 525);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(210, 26);
            this.label3.TabIndex = 5;
            this.label3.Text = "【Is it in the library ?】";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button2.Location = new System.Drawing.Point(759, 257);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(206, 31);
            this.button2.TabIndex = 7;
            this.button2.Text = "③ Run";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.SearchOfRecommendedBooks_Click);
            // 
            // GetMentionsTimeLine
            // 
            this.GetMentionsTimeLine.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.GetMentionsTimeLine.Location = new System.Drawing.Point(854, 38);
            this.GetMentionsTimeLine.Name = "GetMentionsTimeLine";
            this.GetMentionsTimeLine.Size = new System.Drawing.Size(111, 202);
            this.GetMentionsTimeLine.TabIndex = 10;
            this.GetMentionsTimeLine.Text = "① Get MentionsTimeLine";
            this.GetMentionsTimeLine.UseVisualStyleBackColor = true;
            this.GetMentionsTimeLine.Click += new System.EventHandler(this.GetMentionsTimeLine_Click);
            // 
            // MentionsListBox
            // 
            this.MentionsListBox.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.MentionsListBox.FormattingEnabled = true;
            this.MentionsListBox.ItemHeight = 22;
            this.MentionsListBox.Location = new System.Drawing.Point(9, 38);
            this.MentionsListBox.Name = "MentionsListBox";
            this.MentionsListBox.Size = new System.Drawing.Size(829, 202);
            this.MentionsListBox.TabIndex = 11;
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button5.Location = new System.Drawing.Point(685, 524);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(184, 29);
            this.button5.TabIndex = 12;
            this.button5.Text = "Result of Tweets";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.ResultOfTweets_Click);
            // 
            // ScoreOfNegaPosiListBox
            // 
            this.ScoreOfNegaPosiListBox.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ScoreOfNegaPosiListBox.FormattingEnabled = true;
            this.ScoreOfNegaPosiListBox.ItemHeight = 22;
            this.ScoreOfNegaPosiListBox.Location = new System.Drawing.Point(9, 557);
            this.ScoreOfNegaPosiListBox.Name = "ScoreOfNegaPosiListBox";
            this.ScoreOfNegaPosiListBox.Size = new System.Drawing.Size(248, 136);
            this.ScoreOfNegaPosiListBox.TabIndex = 13;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(875, 557);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(93, 136);
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // DisplayResultListBox
            // 
            this.DisplayResultListBox.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DisplayResultListBox.FormattingEnabled = true;
            this.DisplayResultListBox.ItemHeight = 22;
            this.DisplayResultListBox.Location = new System.Drawing.Point(441, 557);
            this.DisplayResultListBox.Name = "DisplayResultListBox";
            this.DisplayResultListBox.Size = new System.Drawing.Size(428, 136);
            this.DisplayResultListBox.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(-7, 522);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(283, 26);
            this.label4.TabIndex = 17;
            this.label4.Text = "【Score of negative-positive】";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(263, 534);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(158, 159);
            this.pictureBox2.TabIndex = 18;
            this.pictureBox2.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("源ノ角ゴシック JP Regular", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(-7, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(319, 26);
            this.label5.TabIndex = 19;
            this.label5.Text = "【Mention of @WebInfoSystems】";
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(977, 703);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.DisplayResultListBox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ScoreOfNegaPosiListBox);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.MentionsListBox);
            this.Controls.Add(this.GetMentionsTimeLine);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SearchBooksListBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.AnalysisListBox);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Recommended Books System";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox AnalysisListBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox SearchBooksListBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button GetMentionsTimeLine;
        private System.Windows.Forms.ListBox MentionsListBox;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.ListBox ScoreOfNegaPosiListBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListBox DisplayResultListBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ImageList imageList1;
    }
}

