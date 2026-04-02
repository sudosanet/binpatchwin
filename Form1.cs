using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace binpatchwin
{
    public partial class Form1 : Form
    {
        private readonly Color _panelNormal = Color.FromArgb(245, 245, 250);
        private readonly Color _panelDragOver = Color.FromArgb(220, 230, 250);
        private readonly Color _placeholderColor = Color.FromArgb(160, 160, 170);
        private readonly Color _pathColor = Color.FromArgb(30, 30, 30);
        private const string PlaceholderText = "ここにファイルをドロップ";

        private string _createOriginalPath = "";
        private string _createModifiedPath = "";
        private string _applyOriginalPath = "";
        private string _applyPatchPath = "";

        public Form1()
        {
            InitializeComponent();
        }

        // === パス設定ヘルパー ===

        private void SetPathLabel(Label label, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                label.Text = PlaceholderText;
                label.ForeColor = _placeholderColor;
            }
            else
            {
                label.Text = path;
                label.ForeColor = _pathColor;
            }
        }

        // === ドラッグ&ドロップ 共通 ===

        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                var p = sender as Panel;
                if (p != null) p.BackColor = _panelDragOver;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Panel_DragLeave(object sender, EventArgs e)
        {
            var p = sender as Panel;
            if (p != null) p.BackColor = _panelNormal;
        }

        private static string GetDroppedFile(DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null && files.Length > 0)
                return files[0];
            return null;
        }

        // === パッチ作成タブ ===

        private void SetCreateOriginal(string path)
        {
            _createOriginalPath = path;
            SetPathLabel(lblCreateOriginalPath, path);
            UpdateCreateOutput();
        }

        private void SetCreateModified(string path)
        {
            _createModifiedPath = path;
            SetPathLabel(lblCreateModifiedPath, path);
        }

        private void UpdateCreateOutput()
        {
            if (!string.IsNullOrWhiteSpace(_createOriginalPath))
                lblCreateOutputPath.Text = _createOriginalPath + ".bpat";
            else
                lblCreateOutputPath.Text = "";
        }

        private void btnCreateOriginalBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Title = "元ファイルを選択", Filter = "すべてのファイル|*.*" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    SetCreateOriginal(dlg.FileName);
            }
        }

        private void panelCreateOriginal_DragDrop(object sender, DragEventArgs e)
        {
            var p = sender as Panel;
            if (p != null) p.BackColor = _panelNormal;
            var file = GetDroppedFile(e);
            if (file != null) SetCreateOriginal(file);
        }

        private void btnCreateModifiedBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Title = "変更後ファイルを選択", Filter = "すべてのファイル|*.*" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    SetCreateModified(dlg.FileName);
            }
        }

        private void panelCreateModified_DragDrop(object sender, DragEventArgs e)
        {
            var p = sender as Panel;
            if (p != null) p.BackColor = _panelNormal;
            var file = GetDroppedFile(e);
            if (file != null) SetCreateModified(file);
        }

        private async void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_createOriginalPath) ||
                string.IsNullOrWhiteSpace(_createModifiedPath))
            {
                MessageBox.Show("元ファイルと変更後ファイルを指定してください。", "入力エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetUIEnabled(false);
            lblStatus.Text = "パッチを作成中...";
            var progress = new Progress<int>(v => progressBar.Value = v);

            try
            {
                var outputPath = _createOriginalPath + ".bpat";
                var patch = await Task.Run(() =>
                    BinaryPatcher.CreatePatch(_createOriginalPath, _createModifiedPath, progress));

                using (var fs = File.Create(outputPath))
                {
                    patch.WriteTo(fs);
                }

                lblStatus.Text = "パッチの作成が完了しました。";
                MessageBox.Show(string.Format("パッチファイルを作成しました。\n{0}", outputPath), "完了",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "エラーが発生しました。";
                MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetUIEnabled(true);
                progressBar.Value = 0;
            }
        }

        // === パッチ適用タブ ===

        private void btnApplyOriginalBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Title = "元ファイルを選択", Filter = "すべてのファイル|*.*" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _applyOriginalPath = dlg.FileName;
                    SetPathLabel(lblApplyOriginalPath, dlg.FileName);
                }
            }
        }

        private void panelApplyOriginal_DragDrop(object sender, DragEventArgs e)
        {
            var p = sender as Panel;
            if (p != null) p.BackColor = _panelNormal;
            var file = GetDroppedFile(e);
            if (file != null)
            {
                _applyOriginalPath = file;
                SetPathLabel(lblApplyOriginalPath, file);
            }
        }

        private void btnApplyPatchBrowse_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog
            {
                Title = "パッチファイルを選択",
                Filter = "パッチファイル (*.bpat)|*.bpat|すべてのファイル|*.*"
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _applyPatchPath = dlg.FileName;
                    SetPathLabel(lblApplyPatchPath, dlg.FileName);
                }
            }
        }

        private void panelApplyPatch_DragDrop(object sender, DragEventArgs e)
        {
            var p = sender as Panel;
            if (p != null) p.BackColor = _panelNormal;
            var file = GetDroppedFile(e);
            if (file != null)
            {
                _applyPatchPath = file;
                SetPathLabel(lblApplyPatchPath, file);
            }
        }

        private async void btnApply_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_applyOriginalPath) ||
                string.IsNullOrWhiteSpace(_applyPatchPath))
            {
                MessageBox.Show("元ファイルとパッチファイルを指定してください。", "入力エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                string.Format("以下のファイルを上書きします。よろしいですか？\n\n{0}", _applyOriginalPath),
                "上書き確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            SetUIEnabled(false);
            lblStatus.Text = "パッチを適用中...";
            var progress = new Progress<int>(v => progressBar.Value = v);

            try
            {
                PatchFile patch;
                using (var fs = File.OpenRead(_applyPatchPath))
                {
                    patch = PatchFile.ReadFrom(fs);
                }

                var tempPath = _applyOriginalPath + ".tmp";
                await Task.Run(() =>
                    BinaryPatcher.ApplyPatch(_applyOriginalPath, patch, tempPath, progress));

                File.Delete(_applyOriginalPath);
                File.Move(tempPath, _applyOriginalPath);

                lblStatus.Text = "パッチの適用が完了しました。";
                MessageBox.Show("パッチを適用しました。元ファイルを上書きしました。", "完了",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "エラーが発生しました。";
                MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetUIEnabled(true);
                progressBar.Value = 0;
            }
        }

        // === 共通 ===

        private void SetUIEnabled(bool enabled)
        {
            tabControl.Enabled = enabled;
        }
    }
}
