using System.Drawing;
using System.Windows.Forms;

namespace binpatchwin
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            tabControl = new TabControl();
            tabCreate = new TabPage();
            tabApply = new TabPage();

            // Create tab controls
            panelCreateOriginal = new Panel();
            lblCreateOriginal = new Label();
            lblCreateOriginalPath = new Label();
            btnCreateOriginalBrowse = new Button();
            panelCreateModified = new Panel();
            lblCreateModified = new Label();
            lblCreateModifiedPath = new Label();
            btnCreateModifiedBrowse = new Button();
            lblCreateOutput = new Label();
            lblCreateOutputPath = new Label();
            btnCreate = new Button();

            // Apply tab controls
            panelApplyOriginal = new Panel();
            lblApplyOriginal = new Label();
            lblApplyOriginalPath = new Label();
            btnApplyOriginalBrowse = new Button();
            panelApplyPatch = new Panel();
            lblApplyPatch = new Label();
            lblApplyPatchPath = new Label();
            btnApplyPatchBrowse = new Button();
            lblApplyInfo = new Label();
            btnApply = new Button();

            // Bottom controls
            progressBar = new ProgressBar();
            lblStatus = new Label();

            var panelColor = Color.FromArgb(245, 245, 250);
            var borderColor = Color.FromArgb(180, 180, 200);
            var accentColor = Color.FromArgb(60, 120, 200);
            var btnFont = new Font("Meiryo UI", 9.5F, FontStyle.Bold);
            var labelFont = new Font("Meiryo UI", 9F);
            var pathFont = new Font("Meiryo UI", 8.5F);
            var placeholderColor = Color.FromArgb(160, 160, 170);
            var placeholderText = "ここにファイルをドロップ";

            // TabControl
            tabControl.Controls.Add(tabCreate);
            tabControl.Controls.Add(tabApply);
            tabControl.Location = new Point(12, 12);
            tabControl.Size = new Size(500, 310);
            tabControl.Font = new Font("Meiryo UI", 9.5F);

            // ==============================
            // Tab: パッチ作成
            // ==============================
            tabCreate.Text = "  パッチ作成  ";
            tabCreate.BackColor = Color.White;
            tabCreate.Padding = new Padding(12);
            tabCreate.Controls.AddRange(new Control[] {
                panelCreateOriginal, lblCreateOriginal, btnCreateOriginalBrowse,
                panelCreateModified, lblCreateModified, btnCreateModifiedBrowse,
                lblCreateOutput, lblCreateOutputPath, btnCreate });

            // 元ファイル (A)
            lblCreateOriginal.Text = "元ファイル (A)";
            lblCreateOriginal.Font = labelFont;
            lblCreateOriginal.Location = new Point(18, 16);
            lblCreateOriginal.AutoSize = true;

            panelCreateOriginal.Location = new Point(18, 36);
            panelCreateOriginal.Size = new Size(370, 32);
            panelCreateOriginal.BackColor = panelColor;
            panelCreateOriginal.BorderStyle = BorderStyle.FixedSingle;
            panelCreateOriginal.AllowDrop = true;
            panelCreateOriginal.DragEnter += Panel_DragEnter;
            panelCreateOriginal.DragDrop += panelCreateOriginal_DragDrop;
            panelCreateOriginal.DragLeave += Panel_DragLeave;

            lblCreateOriginalPath.Text = placeholderText;
            lblCreateOriginalPath.Font = pathFont;
            lblCreateOriginalPath.ForeColor = placeholderColor;
            lblCreateOriginalPath.Location = new Point(6, 6);
            lblCreateOriginalPath.Size = new Size(356, 20);
            lblCreateOriginalPath.AutoEllipsis = true;
            panelCreateOriginal.Controls.Add(lblCreateOriginalPath);

            btnCreateOriginalBrowse.Text = "参照";
            btnCreateOriginalBrowse.Font = new Font("Meiryo UI", 8.5F);
            btnCreateOriginalBrowse.Location = new Point(394, 36);
            btnCreateOriginalBrowse.Size = new Size(70, 32);
            btnCreateOriginalBrowse.FlatStyle = FlatStyle.Flat;
            btnCreateOriginalBrowse.FlatAppearance.BorderColor = borderColor;
            btnCreateOriginalBrowse.Click += btnCreateOriginalBrowse_Click;

            // 変更後ファイル (B)
            lblCreateModified.Text = "変更後ファイル (B)";
            lblCreateModified.Font = labelFont;
            lblCreateModified.Location = new Point(18, 80);
            lblCreateModified.AutoSize = true;

            panelCreateModified.Location = new Point(18, 100);
            panelCreateModified.Size = new Size(370, 32);
            panelCreateModified.BackColor = panelColor;
            panelCreateModified.BorderStyle = BorderStyle.FixedSingle;
            panelCreateModified.AllowDrop = true;
            panelCreateModified.DragEnter += Panel_DragEnter;
            panelCreateModified.DragDrop += panelCreateModified_DragDrop;
            panelCreateModified.DragLeave += Panel_DragLeave;

            lblCreateModifiedPath.Text = placeholderText;
            lblCreateModifiedPath.Font = pathFont;
            lblCreateModifiedPath.ForeColor = placeholderColor;
            lblCreateModifiedPath.Location = new Point(6, 6);
            lblCreateModifiedPath.Size = new Size(356, 20);
            lblCreateModifiedPath.AutoEllipsis = true;
            panelCreateModified.Controls.Add(lblCreateModifiedPath);

            btnCreateModifiedBrowse.Text = "参照";
            btnCreateModifiedBrowse.Font = new Font("Meiryo UI", 8.5F);
            btnCreateModifiedBrowse.Location = new Point(394, 100);
            btnCreateModifiedBrowse.Size = new Size(70, 32);
            btnCreateModifiedBrowse.FlatStyle = FlatStyle.Flat;
            btnCreateModifiedBrowse.FlatAppearance.BorderColor = borderColor;
            btnCreateModifiedBrowse.Click += btnCreateModifiedBrowse_Click;

            // 出力先 (自動)
            lblCreateOutput.Text = "出力パッチファイル (自動設定):";
            lblCreateOutput.Font = labelFont;
            lblCreateOutput.Location = new Point(18, 148);
            lblCreateOutput.AutoSize = true;

            lblCreateOutputPath.Location = new Point(18, 168);
            lblCreateOutputPath.Size = new Size(446, 20);
            lblCreateOutputPath.Font = pathFont;
            lblCreateOutputPath.ForeColor = Color.FromArgb(100, 100, 100);
            lblCreateOutputPath.AutoEllipsis = true;

            // 作成ボタン
            btnCreate.Text = "パッチ作成";
            btnCreate.Font = btnFont;
            btnCreate.Location = new Point(160, 215);
            btnCreate.Size = new Size(160, 40);
            btnCreate.FlatStyle = FlatStyle.Flat;
            btnCreate.BackColor = accentColor;
            btnCreate.ForeColor = Color.White;
            btnCreate.FlatAppearance.BorderSize = 0;
            btnCreate.Cursor = Cursors.Hand;
            btnCreate.Click += btnCreate_Click;

            // ==============================
            // Tab: パッチ適用
            // ==============================
            tabApply.Text = "  パッチ適用  ";
            tabApply.BackColor = Color.White;
            tabApply.Padding = new Padding(12);
            tabApply.Controls.AddRange(new Control[] {
                panelApplyOriginal, lblApplyOriginal, btnApplyOriginalBrowse,
                panelApplyPatch, lblApplyPatch, btnApplyPatchBrowse,
                lblApplyInfo, btnApply });

            // 元ファイル (A)
            lblApplyOriginal.Text = "元ファイル (A) ※上書きされます";
            lblApplyOriginal.Font = labelFont;
            lblApplyOriginal.ForeColor = Color.FromArgb(180, 60, 60);
            lblApplyOriginal.Location = new Point(18, 16);
            lblApplyOriginal.AutoSize = true;

            panelApplyOriginal.Location = new Point(18, 36);
            panelApplyOriginal.Size = new Size(370, 32);
            panelApplyOriginal.BackColor = panelColor;
            panelApplyOriginal.BorderStyle = BorderStyle.FixedSingle;
            panelApplyOriginal.AllowDrop = true;
            panelApplyOriginal.DragEnter += Panel_DragEnter;
            panelApplyOriginal.DragDrop += panelApplyOriginal_DragDrop;
            panelApplyOriginal.DragLeave += Panel_DragLeave;

            lblApplyOriginalPath.Text = placeholderText;
            lblApplyOriginalPath.Font = pathFont;
            lblApplyOriginalPath.ForeColor = placeholderColor;
            lblApplyOriginalPath.Location = new Point(6, 6);
            lblApplyOriginalPath.Size = new Size(356, 20);
            lblApplyOriginalPath.AutoEllipsis = true;
            panelApplyOriginal.Controls.Add(lblApplyOriginalPath);

            btnApplyOriginalBrowse.Text = "参照";
            btnApplyOriginalBrowse.Font = new Font("Meiryo UI", 8.5F);
            btnApplyOriginalBrowse.Location = new Point(394, 36);
            btnApplyOriginalBrowse.Size = new Size(70, 32);
            btnApplyOriginalBrowse.FlatStyle = FlatStyle.Flat;
            btnApplyOriginalBrowse.FlatAppearance.BorderColor = borderColor;
            btnApplyOriginalBrowse.Click += btnApplyOriginalBrowse_Click;

            // パッチファイル
            lblApplyPatch.Text = "パッチファイル (.bpat)";
            lblApplyPatch.Font = labelFont;
            lblApplyPatch.Location = new Point(18, 80);
            lblApplyPatch.AutoSize = true;

            panelApplyPatch.Location = new Point(18, 100);
            panelApplyPatch.Size = new Size(370, 32);
            panelApplyPatch.BackColor = panelColor;
            panelApplyPatch.BorderStyle = BorderStyle.FixedSingle;
            panelApplyPatch.AllowDrop = true;
            panelApplyPatch.DragEnter += Panel_DragEnter;
            panelApplyPatch.DragDrop += panelApplyPatch_DragDrop;
            panelApplyPatch.DragLeave += Panel_DragLeave;

            lblApplyPatchPath.Text = placeholderText;
            lblApplyPatchPath.Font = pathFont;
            lblApplyPatchPath.ForeColor = placeholderColor;
            lblApplyPatchPath.Location = new Point(6, 6);
            lblApplyPatchPath.Size = new Size(356, 20);
            lblApplyPatchPath.AutoEllipsis = true;
            panelApplyPatch.Controls.Add(lblApplyPatchPath);

            btnApplyPatchBrowse.Text = "参照";
            btnApplyPatchBrowse.Font = new Font("Meiryo UI", 8.5F);
            btnApplyPatchBrowse.Location = new Point(394, 100);
            btnApplyPatchBrowse.Size = new Size(70, 32);
            btnApplyPatchBrowse.FlatStyle = FlatStyle.Flat;
            btnApplyPatchBrowse.FlatAppearance.BorderColor = borderColor;
            btnApplyPatchBrowse.Click += btnApplyPatchBrowse_Click;

            // 上書き情報
            lblApplyInfo.Text = "※ パッチ適用により元ファイルが直接上書きされます";
            lblApplyInfo.Font = new Font("Meiryo UI", 8.5F);
            lblApplyInfo.ForeColor = Color.FromArgb(140, 140, 140);
            lblApplyInfo.Location = new Point(18, 150);
            lblApplyInfo.AutoSize = true;

            // 適用ボタン
            btnApply.Text = "パッチ適用";
            btnApply.Font = btnFont;
            btnApply.Location = new Point(160, 215);
            btnApply.Size = new Size(160, 40);
            btnApply.FlatStyle = FlatStyle.Flat;
            btnApply.BackColor = Color.FromArgb(200, 80, 60);
            btnApply.ForeColor = Color.White;
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Cursor = Cursors.Hand;
            btnApply.Click += btnApply_Click;

            // ProgressBar
            progressBar.Location = new Point(12, 330);
            progressBar.Size = new Size(500, 18);
            progressBar.Style = ProgressBarStyle.Continuous;

            // Status Label
            lblStatus.Text = "準備完了";
            lblStatus.Font = new Font("Meiryo UI", 8.5F);
            lblStatus.ForeColor = Color.FromArgb(100, 100, 100);
            lblStatus.Location = new Point(12, 354);
            lblStatus.AutoSize = true;

            // Form
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(524, 380);
            Controls.Add(tabControl);
            Controls.Add(progressBar);
            Controls.Add(lblStatus);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(240, 240, 245);
            Text = "バイナリパッチ作成・適用ツール";
        }

        #endregion

        private TabControl tabControl;
        private TabPage tabCreate;
        private TabPage tabApply;

        private Panel panelCreateOriginal;
        private Label lblCreateOriginal;
        private Label lblCreateOriginalPath;
        private Button btnCreateOriginalBrowse;
        private Panel panelCreateModified;
        private Label lblCreateModified;
        private Label lblCreateModifiedPath;
        private Button btnCreateModifiedBrowse;
        private Label lblCreateOutput;
        private Label lblCreateOutputPath;
        private Button btnCreate;

        private Panel panelApplyOriginal;
        private Label lblApplyOriginal;
        private Label lblApplyOriginalPath;
        private Button btnApplyOriginalBrowse;
        private Panel panelApplyPatch;
        private Label lblApplyPatch;
        private Label lblApplyPatchPath;
        private Button btnApplyPatchBrowse;
        private Label lblApplyInfo;
        private Button btnApply;

        private ProgressBar progressBar;
        private Label lblStatus;
    }
}
