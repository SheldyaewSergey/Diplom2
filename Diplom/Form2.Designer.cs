namespace Diplom
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.labelVozvrat = new System.Windows.Forms.Label();
            this.labelExit = new System.Windows.Forms.Label();
            this.labelOst = new System.Windows.Forms.Label();
            this.labelZakaz = new System.Windows.Forms.Label();
            this.labelPriem = new System.Windows.Forms.Label();
            this.labelVidacha = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1Moi = new System.Windows.Forms.Label();
            this.labelSklad = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.groupBox1.Controls.Add(this.contentPanel);
            this.groupBox1.Controls.Add(this.labelVozvrat);
            this.groupBox1.Controls.Add(this.labelExit);
            this.groupBox1.Controls.Add(this.labelOst);
            this.groupBox1.Controls.Add(this.labelZakaz);
            this.groupBox1.Controls.Add(this.labelPriem);
            this.groupBox1.Controls.Add(this.labelVidacha);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.label1Moi);
            this.groupBox1.Controls.Add(this.labelSklad);
            this.groupBox1.Location = new System.Drawing.Point(-1, -10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(211, 669);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // contentPanel
            // 
            this.contentPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.contentPanel.Location = new System.Drawing.Point(212, 10);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(850, 660);
            this.contentPanel.TabIndex = 5;
            // 
            // labelVozvrat
            // 
            this.labelVozvrat.AutoSize = true;
            this.labelVozvrat.Font = new System.Drawing.Font("Ebrima", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelVozvrat.Location = new System.Drawing.Point(6, 269);
            this.labelVozvrat.Name = "labelVozvrat";
            this.labelVozvrat.Size = new System.Drawing.Size(186, 30);
            this.labelVozvrat.TabIndex = 18;
            this.labelVozvrat.Text = "Принять возврат";
            this.labelVozvrat.Click += new System.EventHandler(this.labelVozvrat_Click);
            // 
            // labelExit
            // 
            this.labelExit.AutoSize = true;
            this.labelExit.Font = new System.Drawing.Font("Ebrima", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelExit.Location = new System.Drawing.Point(51, 629);
            this.labelExit.Name = "labelExit";
            this.labelExit.Size = new System.Drawing.Size(78, 30);
            this.labelExit.TabIndex = 15;
            this.labelExit.Text = "Выйти";
            this.labelExit.Click += new System.EventHandler(this.labelExit_Click);
            // 
            // labelOst
            // 
            this.labelOst.AutoSize = true;
            this.labelOst.Font = new System.Drawing.Font("Ebrima", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelOst.Location = new System.Drawing.Point(6, 483);
            this.labelOst.Name = "labelOst";
            this.labelOst.Size = new System.Drawing.Size(188, 30);
            this.labelOst.TabIndex = 15;
            this.labelOst.Text = "Таблица товаров";
            this.labelOst.Click += new System.EventHandler(this.labelOst_Click);
            // 
            // labelZakaz
            // 
            this.labelZakaz.AutoSize = true;
            this.labelZakaz.Font = new System.Drawing.Font("Ebrima", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelZakaz.Location = new System.Drawing.Point(6, 437);
            this.labelZakaz.Name = "labelZakaz";
            this.labelZakaz.Size = new System.Drawing.Size(187, 30);
            this.labelZakaz.TabIndex = 14;
            this.labelZakaz.Text = "Таблица заказов";
            this.labelZakaz.Click += new System.EventHandler(this.labelZakaz_Click);
            // 
            // labelPriem
            // 
            this.labelPriem.AutoSize = true;
            this.labelPriem.Font = new System.Drawing.Font("Ebrima", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelPriem.Location = new System.Drawing.Point(6, 224);
            this.labelPriem.Name = "labelPriem";
            this.labelPriem.Size = new System.Drawing.Size(196, 30);
            this.labelPriem.TabIndex = 13;
            this.labelPriem.Text = "Принять поставку";
            this.labelPriem.Click += new System.EventHandler(this.labelPriem_Click);
            // 
            // labelVidacha
            // 
            this.labelVidacha.AutoSize = true;
            this.labelVidacha.Font = new System.Drawing.Font("Ebrima", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelVidacha.Location = new System.Drawing.Point(6, 184);
            this.labelVidacha.Name = "labelVidacha";
            this.labelVidacha.Size = new System.Drawing.Size(166, 30);
            this.labelVidacha.TabIndex = 5;
            this.labelVidacha.Text = "Выдать заказа";
            this.labelVidacha.Click += new System.EventHandler(this.labelVidacha_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(97, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label1Moi
            // 
            this.label1Moi.AutoSize = true;
            this.label1Moi.Font = new System.Drawing.Font("Ebrima", 21F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1Moi.Location = new System.Drawing.Point(23, 19);
            this.label1Moi.Name = "label1Moi";
            this.label1Moi.Size = new System.Drawing.Size(80, 38);
            this.label1Moi.TabIndex = 10;
            this.label1Moi.Text = "Мой";
            this.label1Moi.Click += new System.EventHandler(this.label1Moi_Click);
            // 
            // labelSklad
            // 
            this.labelSklad.AutoSize = true;
            this.labelSklad.Font = new System.Drawing.Font("Ebrima", 21F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelSklad.Location = new System.Drawing.Point(60, 49);
            this.labelSklad.Name = "labelSklad";
            this.labelSklad.Size = new System.Drawing.Size(105, 38);
            this.labelSklad.TabIndex = 11;
            this.labelSklad.Text = "склад";
            this.labelSklad.Click += new System.EventHandler(this.labelSklad_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1064, 659);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Ebrima", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "Главное меню";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1Moi;
        private System.Windows.Forms.Label labelSklad;
        private System.Windows.Forms.Label labelZakaz;
        private System.Windows.Forms.Label labelPriem;
        private System.Windows.Forms.Label labelVidacha;
        private System.Windows.Forms.Label labelOst;
        private System.Windows.Forms.Label labelExit;
        private System.Windows.Forms.Label labelVozvrat;
        private System.Windows.Forms.Panel contentPanel;
    }
}