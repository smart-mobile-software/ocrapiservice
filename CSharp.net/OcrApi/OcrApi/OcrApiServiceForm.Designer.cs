namespace OcrApi
{
    partial class OcrApiServiceForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._sendButton = new System.Windows.Forms.Button();
            this._apiCodeLabel = new System.Windows.Forms.Label();
            this._apiCodeTextBox = new System.Windows.Forms.TextBox();
            this._languageCodeComboBox = new System.Windows.Forms.ComboBox();
            this._languageCodeLabel = new System.Windows.Forms.Label();
            this._browseButton = new System.Windows.Forms.Button();
            this._selectFileTextBox = new System.Windows.Forms.TextBox();
            this._selectFileLabel = new System.Windows.Forms.Label();
            this._requestGroupBox = new System.Windows.Forms.GroupBox();
            this._responseGroupBox = new System.Windows.Forms.GroupBox();
            this._responseTextBox = new System.Windows.Forms.TextBox();
            this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this._panel = new System.Windows.Forms.Panel();
            this._requestGroupBox.SuspendLayout();
            this._responseGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
            this._panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _sendButton
            // 
            this._sendButton.Location = new System.Drawing.Point(15, 313);
            this._sendButton.Name = "_sendButton";
            this._sendButton.Size = new System.Drawing.Size(75, 23);
            this._sendButton.TabIndex = 0;
            this._sendButton.Text = global::OcrApi.ControlsResource.SEND_BUTTON;
            this._sendButton.UseVisualStyleBackColor = true;
            this._sendButton.Click += new System.EventHandler(this._sendButton_Click);
            // 
            // _apiCodeLabel
            // 
            this._apiCodeLabel.AutoSize = true;
            this._apiCodeLabel.Location = new System.Drawing.Point(12, 20);
            this._apiCodeLabel.Name = "_apiCodeLabel";
            this._apiCodeLabel.Size = new System.Drawing.Size(50, 13);
            this._apiCodeLabel.TabIndex = 1;
            this._apiCodeLabel.Text = "Api Code";
            // 
            // _apiCodeTextBox
            // 
            this._apiCodeTextBox.Location = new System.Drawing.Point(32, 36);
            this._apiCodeTextBox.Name = "_apiCodeTextBox";
            this._apiCodeTextBox.Size = new System.Drawing.Size(196, 20);
            this._apiCodeTextBox.TabIndex = 2;
            this._apiCodeTextBox.TextChanged += new System.EventHandler(this._apiCodeTextBox_TextChanged);
            // 
            // _languageCodeComboBox
            // 
            this._languageCodeComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this._languageCodeComboBox.Location = new System.Drawing.Point(32, 76);
            this._languageCodeComboBox.Name = "_languageCodeComboBox";
            this._languageCodeComboBox.Size = new System.Drawing.Size(196, 21);
            this._languageCodeComboBox.TabIndex = 4;
            this._languageCodeComboBox.TextChanged += new System.EventHandler(this._languageCodeTextBox_TextChanged);
            // 
            // _languageCodeLabel
            // 
            this._languageCodeLabel.AutoSize = true;
            this._languageCodeLabel.Location = new System.Drawing.Point(12, 60);
            this._languageCodeLabel.Name = "_languageCodeLabel";
            this._languageCodeLabel.Size = new System.Drawing.Size(55, 13);
            this._languageCodeLabel.TabIndex = 3;
            this._languageCodeLabel.Text = "Language";
            // 
            // _browseButton
            // 
            this._browseButton.Location = new System.Drawing.Point(234, 115);
            this._browseButton.Name = "_browseButton";
            this._browseButton.Size = new System.Drawing.Size(75, 23);
            this._browseButton.TabIndex = 5;
            this._browseButton.Text = global::OcrApi.ControlsResource.BROWSE_BUTTON;
            this._browseButton.UseVisualStyleBackColor = true;
            this._browseButton.Click += new System.EventHandler(this._browseButton_Click);
            // 
            // _selectFileTextBox
            // 
            this._selectFileTextBox.Location = new System.Drawing.Point(32, 117);
            this._selectFileTextBox.Name = "_selectFileTextBox";
            this._selectFileTextBox.Size = new System.Drawing.Size(196, 20);
            this._selectFileTextBox.TabIndex = 6;
            this._selectFileTextBox.TextChanged += new System.EventHandler(this._selectFileTextBox_TextChanged);
            this._selectFileTextBox.Validating += new System.ComponentModel.CancelEventHandler(this._selectFileTextBox_Validating);
            // 
            // _selectFileLabel
            // 
            this._selectFileLabel.AutoSize = true;
            this._selectFileLabel.Location = new System.Drawing.Point(11, 101);
            this._selectFileLabel.Name = "_selectFileLabel";
            this._selectFileLabel.Size = new System.Drawing.Size(74, 13);
            this._selectFileLabel.TabIndex = 7;
            this._selectFileLabel.Text = "Image to OCR";
            // 
            // _requestGroupBox
            // 
            this._requestGroupBox.Controls.Add(this._browseButton);
            this._requestGroupBox.Controls.Add(this._selectFileLabel);
            this._requestGroupBox.Controls.Add(this._apiCodeLabel);
            this._requestGroupBox.Controls.Add(this._selectFileTextBox);
            this._requestGroupBox.Controls.Add(this._apiCodeTextBox);
            this._requestGroupBox.Controls.Add(this._languageCodeLabel);
            this._requestGroupBox.Controls.Add(this._languageCodeComboBox);
            this._requestGroupBox.Location = new System.Drawing.Point(15, 12);
            this._requestGroupBox.Name = "_requestGroupBox";
            this._requestGroupBox.Size = new System.Drawing.Size(320, 295);
            this._requestGroupBox.TabIndex = 8;
            this._requestGroupBox.TabStop = false;
            this._requestGroupBox.Text = "groupBox1";
            // 
            // _responseGroupBox
            // 
            this._responseGroupBox.Controls.Add(this._responseTextBox);
            this._responseGroupBox.Location = new System.Drawing.Point(342, 12);
            this._responseGroupBox.Name = "_responseGroupBox";
            this._responseGroupBox.Size = new System.Drawing.Size(314, 295);
            this._responseGroupBox.TabIndex = 9;
            this._responseGroupBox.TabStop = false;
            this._responseGroupBox.Text = "groupBox2";
            // 
            // _responseTextBox
            // 
            this._responseTextBox.Location = new System.Drawing.Point(7, 20);
            this._responseTextBox.Multiline = true;
            this._responseTextBox.Name = "_responseTextBox";
            this._responseTextBox.ReadOnly = true;
            this._responseTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._responseTextBox.Size = new System.Drawing.Size(301, 268);
            this._responseTextBox.TabIndex = 0;
            // 
            // _errorProvider
            // 
            this._errorProvider.ContainerControl = this;
            // 
            // _panel
            // 
            this._panel.Controls.Add(this._responseGroupBox);
            this._panel.Controls.Add(this._requestGroupBox);
            this._panel.Controls.Add(this._sendButton);
            this._panel.Location = new System.Drawing.Point(2, 9);
            this._panel.Name = "_panel";
            this._panel.Size = new System.Drawing.Size(666, 339);
            this._panel.TabIndex = 10;
            // 
            // OcrApiServiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 353);
            this.Controls.Add(this._panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "OcrApiServiceForm";
            this.Text = "Online OCR Api";
            this._requestGroupBox.ResumeLayout(false);
            this._requestGroupBox.PerformLayout();
            this._responseGroupBox.ResumeLayout(false);
            this._responseGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
            this._panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _sendButton;
        private System.Windows.Forms.Label _apiCodeLabel;
        private System.Windows.Forms.TextBox _apiCodeTextBox;
        private System.Windows.Forms.ComboBox _languageCodeComboBox;
        private System.Windows.Forms.Label _languageCodeLabel;
        private System.Windows.Forms.Button _browseButton;
        private System.Windows.Forms.TextBox _selectFileTextBox;
        private System.Windows.Forms.Label _selectFileLabel;
        private System.Windows.Forms.GroupBox _requestGroupBox;
        private System.Windows.Forms.GroupBox _responseGroupBox;
        private System.Windows.Forms.TextBox _responseTextBox;
        private System.Windows.Forms.ErrorProvider _errorProvider;
        private System.Windows.Forms.Panel _panel;
    }
}

