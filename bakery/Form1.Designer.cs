namespace bakery
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkedListBoxBakeryItems = new System.Windows.Forms.CheckedListBox();
            this.checkedListBoxSuppliers = new System.Windows.Forms.CheckedListBox();
            this.btnGenerateProductUsage = new System.Windows.Forms.Button();
            this.btnGenerateSupplierReport = new System.Windows.Forms.Button();
            this.header = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimePickerStart
            // 
            this.dateTimePickerStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dateTimePickerStart.Location = new System.Drawing.Point(42, 158);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new System.Drawing.Size(300, 39);
            this.dateTimePickerStart.TabIndex = 0;
            // 
            // dateTimePickerEnd
            // 
            this.dateTimePickerEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dateTimePickerEnd.Location = new System.Drawing.Point(46, 264);
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.Size = new System.Drawing.Size(300, 39);
            this.dateTimePickerEnd.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(36, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "Начало периода";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(36, 210);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(216, 32);
            this.label2.TabIndex = 3;
            this.label2.Text = "Конец периода";
            // 
            // checkedListBoxBakeryItems
            // 
            this.checkedListBoxBakeryItems.FormattingEnabled = true;
            this.checkedListBoxBakeryItems.Location = new System.Drawing.Point(876, 67);
            this.checkedListBoxBakeryItems.Name = "checkedListBoxBakeryItems";
            this.checkedListBoxBakeryItems.Size = new System.Drawing.Size(253, 602);
            this.checkedListBoxBakeryItems.TabIndex = 5;
            this.checkedListBoxBakeryItems.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxBakeryItems_SelectedIndexChanged);
            // 
            // checkedListBoxSuppliers
            // 
            this.checkedListBoxSuppliers.FormattingEnabled = true;
            this.checkedListBoxSuppliers.Location = new System.Drawing.Point(1170, 67);
            this.checkedListBoxSuppliers.Name = "checkedListBoxSuppliers";
            this.checkedListBoxSuppliers.Size = new System.Drawing.Size(253, 602);
            this.checkedListBoxSuppliers.TabIndex = 7;
            // 
            // btnGenerateProductUsage
            // 
            this.btnGenerateProductUsage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnGenerateProductUsage.Location = new System.Drawing.Point(876, 705);
            this.btnGenerateProductUsage.Name = "btnGenerateProductUsage";
            this.btnGenerateProductUsage.Size = new System.Drawing.Size(253, 142);
            this.btnGenerateProductUsage.TabIndex = 8;
            this.btnGenerateProductUsage.Text = "Сформировать таблицу по использованию продуктов";
            this.btnGenerateProductUsage.UseVisualStyleBackColor = true;
            this.btnGenerateProductUsage.Click += new System.EventHandler(this.btnGenerateProductUsage_Click);
            // 
            // btnGenerateSupplierReport
            // 
            this.btnGenerateSupplierReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnGenerateSupplierReport.Location = new System.Drawing.Point(1170, 705);
            this.btnGenerateSupplierReport.Name = "btnGenerateSupplierReport";
            this.btnGenerateSupplierReport.Size = new System.Drawing.Size(253, 142);
            this.btnGenerateSupplierReport.TabIndex = 9;
            this.btnGenerateSupplierReport.Text = "Сформировать отчет по поставкам";
            this.btnGenerateSupplierReport.UseVisualStyleBackColor = true;
            this.btnGenerateSupplierReport.Click += new System.EventHandler(this.btnGenerateSupplierReport_Click);
            // 
            // header
            // 
            this.header.AutoSize = true;
            this.header.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.header.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.header.Location = new System.Drawing.Point(34, 21);
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(184, 46);
            this.header.TabIndex = 10;
            this.header.Text = "Пекарня";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(36, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(348, 32);
            this.label3.TabIndex = 11;
            this.label3.Text = "Формирование отчетов";
            // 
            // chart1
            // 
            chartArea3.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chart1.Legends.Add(legend3);
            this.chart1.Location = new System.Drawing.Point(426, 21);
            this.chart1.Name = "chart1";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chart1.Series.Add(series3);
            this.chart1.Size = new System.Drawing.Size(429, 282);
            this.chart1.TabIndex = 12;
            this.chart1.Text = "chart1";
            // 
            // tabControl1
            // 
            this.tabControl1.Location = new System.Drawing.Point(42, 325);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(813, 526);
            this.tabControl1.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(876, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(147, 20);
            this.label4.TabIndex = 14;
            this.label4.Text = "Список продуктов";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1166, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(168, 20);
            this.label5.TabIndex = 15;
            this.label5.Text = "Список поставщиков";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1898, 1024);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.header);
            this.Controls.Add(this.checkedListBoxBakeryItems);
            this.Controls.Add(this.btnGenerateSupplierReport);
            this.Controls.Add(this.btnGenerateProductUsage);
            this.Controls.Add(this.checkedListBoxSuppliers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePickerEnd);
            this.Controls.Add(this.dateTimePickerStart);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePickerStart;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox checkedListBoxBakeryItems;
        private System.Windows.Forms.CheckedListBox checkedListBoxSuppliers;
        private System.Windows.Forms.Button btnGenerateProductUsage;
        private System.Windows.Forms.Button btnGenerateSupplierReport;
        private System.Windows.Forms.Label header;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}

