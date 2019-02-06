namespace ClientServerTest.ClientApp
{
    partial class ClientForm
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
            this.grpConnInfo = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblIP = new System.Windows.Forms.Label();
            this.txPort = new System.Windows.Forms.TextBox();
            this.txIP = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.grpMessage = new System.Windows.Forms.GroupBox();
            this.listMessages = new System.Windows.Forms.ListBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.txMessage = new System.Windows.Forms.TextBox();
            this.grpConnInfo.SuspendLayout();
            this.grpMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConnInfo
            // 
            this.grpConnInfo.Controls.Add(this.lblStatus);
            this.grpConnInfo.Controls.Add(this.lblPort);
            this.grpConnInfo.Controls.Add(this.lblIP);
            this.grpConnInfo.Controls.Add(this.txPort);
            this.grpConnInfo.Controls.Add(this.txIP);
            this.grpConnInfo.Controls.Add(this.btnStart);
            this.grpConnInfo.Location = new System.Drawing.Point(12, 12);
            this.grpConnInfo.Name = "grpConnInfo";
            this.grpConnInfo.Size = new System.Drawing.Size(534, 76);
            this.grpConnInfo.TabIndex = 0;
            this.grpConnInfo.TabStop = false;
            this.grpConnInfo.Text = "Client Connection Info";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.lblStatus.Location = new System.Drawing.Point(374, 26);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(137, 24);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Not Connected";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(7, 46);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(26, 13);
            this.lblPort.TabIndex = 4;
            this.lblPort.Text = "Port";
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(7, 20);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(17, 13);
            this.lblIP.TabIndex = 3;
            this.lblIP.Text = "IP";
            // 
            // txPort
            // 
            this.txPort.Location = new System.Drawing.Point(45, 43);
            this.txPort.Name = "txPort";
            this.txPort.Size = new System.Drawing.Size(186, 20);
            this.txPort.TabIndex = 2;
            this.txPort.Text = "3333";
            // 
            // txIP
            // 
            this.txIP.Location = new System.Drawing.Point(45, 17);
            this.txIP.Name = "txIP";
            this.txIP.Size = new System.Drawing.Size(186, 20);
            this.txIP.TabIndex = 1;
            this.txIP.Text = "127.0.0.1";
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnStart.Location = new System.Drawing.Point(249, 17);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(92, 46);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Bağlan";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // grpMessage
            // 
            this.grpMessage.Controls.Add(this.listMessages);
            this.grpMessage.Controls.Add(this.lblMessage);
            this.grpMessage.Controls.Add(this.btnSend);
            this.grpMessage.Controls.Add(this.txMessage);
            this.grpMessage.Location = new System.Drawing.Point(12, 94);
            this.grpMessage.Name = "grpMessage";
            this.grpMessage.Size = new System.Drawing.Size(534, 329);
            this.grpMessage.TabIndex = 6;
            this.grpMessage.TabStop = false;
            this.grpMessage.Text = "Messages";
            // 
            // listMessages
            // 
            this.listMessages.FormattingEnabled = true;
            this.listMessages.Location = new System.Drawing.Point(11, 27);
            this.listMessages.Name = "listMessages";
            this.listMessages.Size = new System.Drawing.Size(517, 264);
            this.listMessages.TabIndex = 7;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(8, 306);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(59, 13);
            this.lblMessage.TabIndex = 6;
            this.lblMessage.Text = "Mesajınız : ";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(454, 301);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 5;
            this.btnSend.Text = "Gönder";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txMessage
            // 
            this.txMessage.Location = new System.Drawing.Point(73, 303);
            this.txMessage.Name = "txMessage";
            this.txMessage.Size = new System.Drawing.Size(375, 20);
            this.txMessage.TabIndex = 4;
            // 
            // ClientForm
            // 
            this.AcceptButton = this.btnSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 450);
            this.Controls.Add(this.grpMessage);
            this.Controls.Add(this.grpConnInfo);
            this.Name = "ClientForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client";
            this.grpConnInfo.ResumeLayout(false);
            this.grpConnInfo.PerformLayout();
            this.grpMessage.ResumeLayout(false);
            this.grpMessage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpConnInfo;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TextBox txPort;
        private System.Windows.Forms.TextBox txIP;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox grpMessage;
        private System.Windows.Forms.ListBox listMessages;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txMessage;
    }
}

