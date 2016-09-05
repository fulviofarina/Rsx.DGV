using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Rsx.DGV
{
    public interface IFind
    {
        string Field { get; set; }
        string Filter { get; }

        void Link(ref System.Data.DataTable dt, ref BindingSource BS);

        void Search();
    }

    public partial class ucSearch : UserControl, Rsx.DGV.IFind
    {
        private DataTable table = null;
        private Form f = null;

        public string Filter
        {
            get
            {
                return this.box.Text;
            }
        }

        public ucSearch()
        {
            InitializeComponent();
            f = new Form();
            f.WindowState = FormWindowState.Normal;
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Size = this.Size;
            f.TopMost = true;
            this.Dock = DockStyle.Fill;
            f.ControlBox = false;
            f.MaximizeBox = false;
            f.MinimizeBox = false;
            // f.AutoSize = true;
            // f.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            f.Controls.Add(this);
            ent = new KeyEventArgs(Keys.Enter);
            esc = new KeyEventArgs(Keys.Escape);
        }

        private BindingSource bs = null;

        public void Link(ref DataTable dt, ref BindingSource BS)
        {
            table = dt;
            IEnumerable<string> ls = table.Columns.OfType<DataColumn>().Select(o => o.ColumnName);
            fieldbox.Items.Clear();
            fieldbox.Items.AddRange(ls.ToArray());
            box.DataSource = table;

            bs = BS;
        }

        public void Search()
        {
            f.Show();
        }

        public static string FilterBS(string field, string filter, ref BindingSource bs, Type typo)
        {
            string msg = string.Empty;

            try
            {
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    if (typo.Equals(typeof(string)))
                    {
                        string def = field + " LIKE ";

                        bs.Filter = def + "'*" + filter + "*'";

                        if (bs.Count == 0)
                        {
                            bs.Filter = def + "'" + filter + "*'";
                        }
                        if (bs.Count == 0)
                        {
                            bs.Filter = def + "'*" + filter + "'";
                        }
                    }
                    else bs.Filter = field + " = '" + filter + "'";
                }
                else bs.Filter = string.Empty;

                if (bs.Count == 0)
                {
                    msg = "Check the filter, nothing was found!";
                }
                else msg = "Filtering... data found!";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        protected KeyEventArgs ent = null;
        protected KeyEventArgs esc = null;

        public string Field
        {
            get { return fieldbox.Text; }
            set { fieldbox.Text = value; }
        }

        private void box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                f.Visible = false;
            }
            else
            {
                // runs the query

                string column = this.fieldbox.Text;

                if (string.IsNullOrWhiteSpace(column)) return;

                f.Opacity = 1;

                Type typo = table.Columns[column].DataType;

                FilterBS(column, this.box.Text, ref bs, typo);
            }
        }

        private void fieldbox_TextChanged(object sender, EventArgs e)
        {
            if (fieldbox.Text.Equals(string.Empty)) return;
            box.TextChanged -= this.box_TextChanged;
            box.DisplayMember = fieldbox.Text;
            box.ValueMember = fieldbox.Text;
            box.TextChanged += this.box_TextChanged;
        }

        private void clr_Click(object sender, EventArgs e)
        {
            this.box.Text = string.Empty;
            box_KeyDown(sender, ent);
        }

        private void close_Click(object sender, EventArgs e)
        {
            clr_Click(sender, e);
            box_KeyDown(sender, esc);
        }

        private void box_TextChanged(object sender, EventArgs e)
        {
            box_KeyDown(sender, ent);
        }
    }
}