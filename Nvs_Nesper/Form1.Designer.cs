namespace Nvs_Nesper
{
    partial class Form1
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
            this.lvMsgIn = new System.Windows.Forms.ListView();
            this.clmhMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ckbShowMsg = new System.Windows.Forms.CheckBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvMsgIn
            // 
            this.lvMsgIn.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmhMessage});
            this.lvMsgIn.FullRowSelect = true;
            this.lvMsgIn.Location = new System.Drawing.Point(3, 35);
            this.lvMsgIn.Name = "lvMsgIn";
            this.lvMsgIn.Size = new System.Drawing.Size(995, 405);
            this.lvMsgIn.TabIndex = 8;
            this.lvMsgIn.UseCompatibleStateImageBehavior = false;
            this.lvMsgIn.View = System.Windows.Forms.View.Details;
            // 
            // clmhMessage
            // 
            this.clmhMessage.Text = "Message";
            this.clmhMessage.Width = 1000;
            // 
            // ckbShowMsg
            // 
            this.ckbShowMsg.AutoSize = true;
            this.ckbShowMsg.Checked = true;
            this.ckbShowMsg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbShowMsg.Location = new System.Drawing.Point(922, 12);
            this.ckbShowMsg.Name = "ckbShowMsg";
            this.ckbShowMsg.Size = new System.Drawing.Size(76, 17);
            this.ckbShowMsg.TabIndex = 9;
            this.ckbShowMsg.Text = "Show Msg";
            this.ckbShowMsg.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.ForeColor = System.Drawing.Color.Blue;
            this.btnStart.Location = new System.Drawing.Point(12, 6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 10;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.ForeColor = System.Drawing.Color.Red;
            this.btnStop.Location = new System.Drawing.Point(93, 6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 11;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 443);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.ckbShowMsg);
            this.Controls.Add(this.lvMsgIn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nesper";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvMsgIn;
        private System.Windows.Forms.ColumnHeader clmhMessage;
        private System.Windows.Forms.CheckBox ckbShowMsg;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
    }
}

