namespace GameController
{
    partial class FormGameController
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
            this.groupBoxRender = new System.Windows.Forms.GroupBox();
            this.comboBoxRenderers = new System.Windows.Forms.ComboBox();
            this.groupBoxMap = new System.Windows.Forms.GroupBox();
            this.buttonSaveMap = new System.Windows.Forms.Button();
            this.buttonLoadMap = new System.Windows.Forms.Button();
            this.groupBoxRender.SuspendLayout();
            this.groupBoxMap.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxRender
            // 
            this.groupBoxRender.Controls.Add(this.comboBoxRenderers);
            this.groupBoxRender.Location = new System.Drawing.Point(12, 12);
            this.groupBoxRender.Name = "groupBoxRender";
            this.groupBoxRender.Size = new System.Drawing.Size(172, 82);
            this.groupBoxRender.TabIndex = 0;
            this.groupBoxRender.TabStop = false;
            this.groupBoxRender.Text = "Render";
            // 
            // comboBoxRenderers
            // 
            this.comboBoxRenderers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRenderers.FormattingEnabled = true;
            this.comboBoxRenderers.Location = new System.Drawing.Point(7, 20);
            this.comboBoxRenderers.Name = "comboBoxRenderers";
            this.comboBoxRenderers.Size = new System.Drawing.Size(159, 21);
            this.comboBoxRenderers.TabIndex = 0;
            // 
            // groupBoxMap
            // 
            this.groupBoxMap.Controls.Add(this.buttonLoadMap);
            this.groupBoxMap.Controls.Add(this.buttonSaveMap);
            this.groupBoxMap.Location = new System.Drawing.Point(12, 101);
            this.groupBoxMap.Name = "groupBoxMap";
            this.groupBoxMap.Size = new System.Drawing.Size(172, 100);
            this.groupBoxMap.TabIndex = 1;
            this.groupBoxMap.TabStop = false;
            this.groupBoxMap.Text = "Map";
            // 
            // buttonSaveMap
            // 
            this.buttonSaveMap.Location = new System.Drawing.Point(7, 20);
            this.buttonSaveMap.Name = "buttonSaveMap";
            this.buttonSaveMap.Size = new System.Drawing.Size(91, 23);
            this.buttonSaveMap.TabIndex = 0;
            this.buttonSaveMap.Text = "Save Map";
            this.buttonSaveMap.UseVisualStyleBackColor = true;
            this.buttonSaveMap.Click += new System.EventHandler(this.buttonSaveMap_Click);
            // 
            // buttonLoadMap
            // 
            this.buttonLoadMap.Location = new System.Drawing.Point(7, 50);
            this.buttonLoadMap.Name = "buttonLoadMap";
            this.buttonLoadMap.Size = new System.Drawing.Size(91, 23);
            this.buttonLoadMap.TabIndex = 1;
            this.buttonLoadMap.Text = "Load Map";
            this.buttonLoadMap.UseVisualStyleBackColor = true;
            this.buttonLoadMap.Click += new System.EventHandler(this.buttonLoadMap_Click);
            // 
            // FormGameController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.groupBoxMap);
            this.Controls.Add(this.groupBoxRender);
            this.Name = "FormGameController";
            this.Text = "Form1";
            this.groupBoxRender.ResumeLayout(false);
            this.groupBoxMap.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxRender;
        private System.Windows.Forms.ComboBox comboBoxRenderers;
        private System.Windows.Forms.GroupBox groupBoxMap;
        private System.Windows.Forms.Button buttonSaveMap;
        private System.Windows.Forms.Button buttonLoadMap;
    }
}

