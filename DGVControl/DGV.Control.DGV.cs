using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Rsx.DGV
{
  public partial class Control
  {
    public delegate bool RowPrePaintChecker(object sender, DataGridViewRowPrePaintEventArgs e);

    public delegate bool RowPostPaintChecker(object sender, DataGridViewRowPostPaintEventArgs e);

    public delegate bool CellPaintChecker(object sender, DataGridViewCellPaintingEventArgs e);

    private DataGridViewCellPaintingEventHandler _paintCells;
    private RowPostPaintChecker _shouldPostPaintRow;
    private RowPrePaintChecker _shouldPrePaintRow;
    private DataGridViewRowPrePaintEventHandler _prepaintRows;
    private DataGridViewRowPostPaintEventHandler _postpaintRows;
    private CellPaintChecker _shouldPaintCell;

    public DataGridViewRowPostPaintEventHandler PostPaintRowsMethod
    {
      get { return _postpaintRows; }
      set { _postpaintRows = value; }
    }

    public DataGridViewRowPrePaintEventHandler PrePaintRowsMethod
    {
      get { return _prepaintRows; }
      set { _prepaintRows = value; }
    }

    public CellPaintChecker ShouldPaintCellMethod
    {
      get { return _shouldPaintCell; }
      set { _shouldPaintCell = value; }
    }

    public RowPostPaintChecker ShouldPostPaintRowMethod
    {
      get { return _shouldPostPaintRow; }
      set { _shouldPostPaintRow = value; }
    }

    public RowPrePaintChecker ShouldPrePaintRowMethod
    {
      get { return _shouldPrePaintRow; }
      set { _shouldPrePaintRow = value; }
    }

    public DataGridViewCellPaintingEventHandler PaintCellsMethod
    {
      get { return _paintCells; }
      set { _paintCells = value; }
    }

    private static void PaintChanges(ref DataGridViewRow r, bool haschanges)
    {
      if (haschanges)
      {
        DataRow dtr = Dumb.Cast<DataRow>(r);

        if (dtr.RowState == DataRowState.Added)
        {
          r.DefaultCellStyle.ForeColor = Color.Blue;
          return;
        }

        if (!dtr.HasVersion(DataRowVersion.Current)) return;
        if (!dtr.HasVersion(DataRowVersion.Original)) return;

        IEnumerable<int> ords = dtr.Table.Columns.OfType<DataColumn>().Select(o => o.Ordinal).ToList();
        IEnumerable<DataGridViewColumn> dgvcols = r.DataGridView.Columns.OfType<DataGridViewColumn>();

        foreach (int x in ords)
        {
          string field2 = dtr.Table.Columns[x].ColumnName; //data table field
          object c = dgvcols.FirstOrDefault(o => o.DataPropertyName.Equals(field2));
          //column that matches field in table
          if (c == null) continue; //column null (not a displayed column)....
          int index = (c as DataGridViewColumn).Index;

          object org = dtr.Field<object>(x, DataRowVersion.Original);
          object curr = dtr.Field<object>(x, DataRowVersion.Current);
          if (org == null || curr == null) continue;
          DataGridViewCell cell = r.Cells[index];
          DataGridViewCellStyle style = cell.Style;
          Color fore = style.ForeColor;
          if (!org.Equals(curr))
          {
            if (fore != Color.Red)
            {
              style.ForeColor = Color.Red;
              r.Cells[index].ToolTipText = "Original value: " + org + "\nCurrent value: " + curr;
            }
          }
          else
          {
            if (fore != Color.Black)
            {
              style.ForeColor = Color.Black;
              r.Cells[index].ToolTipText = string.Empty;
            }
          }
        }
      }
      else
      {
        foreach (DataGridViewCell c in r.Cells)
        {
          Color fore = c.Style.ForeColor;
          if (fore == Color.Red)
          {
            r.Cells[c.ColumnIndex].Style.ForeColor = Color.Black;
            r.Cells[c.ColumnIndex].ToolTipText = string.Empty;
          }
        }
      }
    }

    private void dgv_MouseHover(object sender, EventArgs e)
    {
      DataGridView dgv = sender as DataGridView;
      if (dgv.IsCurrentCellInEditMode) return;

      if (saveButton != null)
      {
        if (saveButton.Enabled) return;
      }
      else if (tsSaveButton != null)
      {
        if (tsSaveButton.Enabled) return;
      }
      else return;

      DataTable dt = GetDataSource<DataTable>(ref dgv);
      if (dt == null) return;
      bool haschanges = Dumb.HasChanges(dt.AsEnumerable());
      SetSaveButtonState(haschanges);
    }

    private void dgv_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
    {
      if (e.RowIndex < 0) return;

      if (!_shouldPostPaintRow(sender, e)) return;

      _postpaintRows(sender, e);
    }

    private void dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      if (e.RowIndex < 0) return;
      //  if(e.State!= DataGridViewElementStates.Visible) return;
      //  DataGridView dgv = sender as DataGridView;
      //  DataGridViewCell cell = dgv[e.ColumnIndex,e.RowIndex];
      //  if (!cell.IsInEditMode) return;

      if (!_shouldPaintCell(sender, e)) return;

      _paintCells(sender, e);
    }

    private void dgv_RowValidated(object sender, DataGridViewCellEventArgs e)
    {
      int ind = e.RowIndex;

      if (ind < 0) return;
      try
      {
        DataGridView dgv = sender as DataGridView;

        DataGridViewRow dgvr = dgv.Rows[ind];
        DataRow r = Dumb.Cast<DataRow>(dgvr);
        if (r == null) return;

        UserControl con = this.usrControl as UserControl;
        con.Validate();

        bool haschanges = Dumb.HasChanges(r);
        PaintChanges(ref dgvr, haschanges);

        if (haschanges) SetSaveButtonState(haschanges); //only if this has changes! otherwise it overrides a previous row that does have canges
      }
      catch (SystemException ex)
      {
      }
    }

    private void dgv_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
    {
      if (e.RowIndex < 0) return;

      if (!_shouldPrePaintRow(sender, e)) return;

      _prepaintRows(sender, e);
    }

    private void dgv_Sorted(object sender, EventArgs e)
    {
      DataGridView dgv = sender as DataGridView;
      string order = " asc";
      if (dgv.SortOrder == SortOrder.Descending) order = " desc";
      BindingSource bs = GetDataSource<BindingSource>(ref dgv);
      bs.Sort = dgv.SortedColumn.DataPropertyName + order;
    }
  }
}