namespace FMMidi
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
            this.label1 = new System.Windows.Forms.Label();
            this.ComboBoxAPISelection = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ComboBoxOutputSelection = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ComboBoxMidiInSelection = new System.Windows.Forms.ComboBox();
            this.ButtonConnect = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "使用するAPI";
            // 
            // ComboBoxAPISelection
            // 
            this.ComboBoxAPISelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxAPISelection.FormattingEnabled = true;
            this.ComboBoxAPISelection.Items.AddRange(new object[] {
            "WaveOut",
            "DirectSound",
            "WASAPI",
            "ASIO"});
            this.ComboBoxAPISelection.Location = new System.Drawing.Point(165, 6);
            this.ComboBoxAPISelection.Name = "ComboBoxAPISelection";
            this.ComboBoxAPISelection.Size = new System.Drawing.Size(398, 20);
            this.ComboBoxAPISelection.TabIndex = 1;
            this.ComboBoxAPISelection.SelectedIndexChanged += new System.EventHandler(this.ComboBoxAPISelection_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "出力デバイス";
            // 
            // ComboBoxOutputSelection
            // 
            this.ComboBoxOutputSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxOutputSelection.FormattingEnabled = true;
            this.ComboBoxOutputSelection.Location = new System.Drawing.Point(165, 32);
            this.ComboBoxOutputSelection.Name = "ComboBoxOutputSelection";
            this.ComboBoxOutputSelection.Size = new System.Drawing.Size(398, 20);
            this.ComboBoxOutputSelection.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "MIDI IN";
            // 
            // ComboBoxMidiInSelection
            // 
            this.ComboBoxMidiInSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxMidiInSelection.FormattingEnabled = true;
            this.ComboBoxMidiInSelection.Location = new System.Drawing.Point(165, 58);
            this.ComboBoxMidiInSelection.Name = "ComboBoxMidiInSelection";
            this.ComboBoxMidiInSelection.Size = new System.Drawing.Size(398, 20);
            this.ComboBoxMidiInSelection.TabIndex = 5;
            // 
            // ButtonConnect
            // 
            this.ButtonConnect.Location = new System.Drawing.Point(569, 6);
            this.ButtonConnect.Name = "ButtonConnect";
            this.ButtonConnect.Size = new System.Drawing.Size(250, 71);
            this.ButtonConnect.TabIndex = 6;
            this.ButtonConnect.Text = "接続";
            this.ButtonConnect.UseVisualStyleBackColor = true;
            this.ButtonConnect.Click += new System.EventHandler(this.ButtonConnect_Click);
            // 
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBox1.Location = new System.Drawing.Point(14, 84);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(805, 283);
            this.textBox1.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(831, 379);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.ButtonConnect);
            this.Controls.Add(this.ComboBoxMidiInSelection);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ComboBoxOutputSelection);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ComboBoxAPISelection);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Kb10uy.Audio.Synthesis.FM MIDIテスト";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ComboBoxAPISelection;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ComboBoxOutputSelection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ComboBoxMidiInSelection;
        private System.Windows.Forms.Button ButtonConnect;
        private System.Windows.Forms.TextBox textBox1;
    }
}

