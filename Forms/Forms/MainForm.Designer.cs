namespace Forms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            WriteDataButton = new Button();
            DeleteData = new Button();
            QueryLabel = new Label();
            checkPassButton = new Button();
            AdminButton = new Button();
            PasswordBox = new TextBox();
            QueryComboBox = new ComboBox();
            DeleteQuery = new Button();
            CitiesComboBox = new ComboBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // WriteDataButton
            // 
            WriteDataButton.Enabled = false;
            WriteDataButton.Location = new Point(235, 99);
            WriteDataButton.Name = "WriteDataButton";
            WriteDataButton.Size = new Size(121, 23);
            WriteDataButton.TabIndex = 3;
            WriteDataButton.Text = "Создать Excel файл";
            WriteDataButton.UseVisualStyleBackColor = true;
            WriteDataButton.Click += WriteData_Click;
            // 
            // DeleteData
            // 
            DeleteData.Location = new Point(11, 133);
            DeleteData.Name = "DeleteData";
            DeleteData.Size = new Size(163, 42);
            DeleteData.TabIndex = 7;
            DeleteData.Text = "Отчистить базу данных вакансий";
            DeleteData.UseVisualStyleBackColor = true;
            DeleteData.Visible = false;
            DeleteData.Click += DeleteData_Click;
            // 
            // QueryLabel
            // 
            QueryLabel.AutoSize = true;
            QueryLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            QueryLabel.Location = new Point(11, 76);
            QueryLabel.Name = "QueryLabel";
            QueryLabel.Size = new Size(225, 21);
            QueryLabel.TabIndex = 0;
            QueryLabel.Text = "Запросы для поиска вакансий";
            // 
            // checkPassButton
            // 
            checkPassButton.Location = new Point(271, 177);
            checkPassButton.Name = "checkPassButton";
            checkPassButton.Size = new Size(54, 23);
            checkPassButton.TabIndex = 6;
            checkPassButton.Text = "Ok";
            checkPassButton.UseVisualStyleBackColor = true;
            checkPassButton.Visible = false;
            checkPassButton.Click += checkPassButton_Click;
            // 
            // AdminButton
            // 
            AdminButton.Location = new Point(103, 133);
            AdminButton.Name = "AdminButton";
            AdminButton.Size = new Size(162, 31);
            AdminButton.TabIndex = 4;
            AdminButton.Text = "Войти как администратор";
            AdminButton.UseVisualStyleBackColor = true;
            AdminButton.Click += AdminButton_Click;
            // 
            // PasswordBox
            // 
            PasswordBox.Location = new Point(103, 178);
            PasswordBox.Name = "PasswordBox";
            PasswordBox.Size = new Size(162, 23);
            PasswordBox.TabIndex = 5;
            PasswordBox.Visible = false;
            // 
            // QueryComboBox
            // 
            QueryComboBox.FormattingEnabled = true;
            QueryComboBox.Location = new Point(11, 100);
            QueryComboBox.MaxDropDownItems = 5;
            QueryComboBox.Name = "QueryComboBox";
            QueryComboBox.Size = new Size(218, 23);
            QueryComboBox.TabIndex = 2;
            QueryComboBox.TextChanged += QueryComboBox_TextChanged;
            // 
            // DeleteQuery
            // 
            DeleteQuery.BackgroundImageLayout = ImageLayout.None;
            DeleteQuery.Location = new Point(193, 133);
            DeleteQuery.Name = "DeleteQuery";
            DeleteQuery.Size = new Size(163, 42);
            DeleteQuery.TabIndex = 8;
            DeleteQuery.Text = "Отчистить базу данных запросов";
            DeleteQuery.UseVisualStyleBackColor = true;
            DeleteQuery.Visible = false;
            DeleteQuery.Click += DeleteQuery_Click;
            // 
            // CitiesComboBox
            // 
            CitiesComboBox.FormattingEnabled = true;
            CitiesComboBox.Location = new Point(11, 44);
            CitiesComboBox.MaxDropDownItems = 5;
            CitiesComboBox.Name = "CitiesComboBox";
            CitiesComboBox.Size = new Size(346, 23);
            CitiesComboBox.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(11, 19);
            label1.Name = "label1";
            label1.Size = new Size(327, 21);
            label1.TabIndex = 13;
            label1.Text = "Добавить свой город, которого нет в списке";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(369, 307);
            Controls.Add(label1);
            Controls.Add(CitiesComboBox);
            Controls.Add(DeleteQuery);
            Controls.Add(QueryComboBox);
            Controls.Add(PasswordBox);
            Controls.Add(AdminButton);
            Controls.Add(checkPassButton);
            Controls.Add(QueryLabel);
            Controls.Add(DeleteData);
            Controls.Add(WriteDataButton);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "Анализ рынка труда";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button WriteDataButton;
        private Button DeleteData;
        private Label QueryLabel;
        private Button checkPassButton;
        private Button AdminButton;
        private TextBox PasswordBox;
        private ComboBox QueryComboBox;
        private Button DeleteQuery;
        private ComboBox CitiesComboBox;
        private Label label1;
    }
}