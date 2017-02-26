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
    public Control(Refresher refresher, Informer informMethod, ref Rsx.DGV.IFind searcher)
    {
      _informMethod = informMethod;

      _refreshMethod = refresher;

      OFD = new OpenFileDialog();
      SFD = new SaveFileDialog();
      this.OFD.DefaultExt = "xml";
      this.OFD.FileName = "File";
      this.OFD.Filter = "XML Tables | *.xml";
      this.OFD.Title = "Import Table File";

      //
      // SFD
      //
      this.SFD.DefaultExt = "xml";
      this.SFD.FileName = "File";
      this.SFD.Filter = "XML Tables | *.xml";
      this.SFD.Title = "Export Table File";

      SFD.FileOk += SFD_FileOk;
      OFD.FileOk += OFD_FileOk;

      search = searcher;
    }

    /// <summary>
    /// Any method of (object sender, EventArgs e) type that will show the result of the process and must be provided.
    /// </summary>
    public Informer InformMethod
    {
      get { return _informMethod; }
      set { _informMethod = value; }
    }

    private Informer _informMethod;

    private string action;
    private string result;

    /// <summary>
    /// Gives feedback about the last result of a given action
    /// </summary>
    public string LastResult
    {
      get { return result; }
    }

    /// <summary>
    /// Gives feedback about the last performed action
    /// </summary>
    public string LastAction
    {
      get { return action; }
    }

    /// <summary>
    /// Makes Copy, Paste, Clone, Undo and Save available to the given input array of datagridviews.
    /// </summary>
    /// <param name="saveMethod">Any method of (object sender, EventArgs e) type that performs the saving code. Must be provided.</param>
    /// <param name="dgvs">Array of datagridviews to implement the multiple features</param>
    /// <param name="informMethod">Any method of (object sender, EventArgs e) type that will show the result of the process and must be provided.</param>
    public DataGridView[] DGVs
    {
      get { return dGVs; }
      set { dGVs = value; }
    }

    private IEnumerable<DataGridView> shortdGVs;
    private DataGridView[] dGVs;

    private object usrControl = null;

    public object UsrControl
    {
      get { return usrControl; }
      set { usrControl = value; }
    }

    private Rsx.DGV.IFind search = null;

    public void SetContext(string controlHeader, ref DataGridView[] dgvs, ContextMenuStrip cms)
    {
      foreach (DataGridView dgv in dgvs)
      {
        dgv.Name = controlHeader; //control header where it is coming from
        dgv.ContextMenuStrip = cms;
        dgv.Tag = this;  //set the cv as tag of the DGV
      }
    }

    public void CreateEvents(ref ToolStripButton[] items)
    {
      ToolStripButton[] items2 = items.Where(i => i.Text.Contains("Save Data")).ToArray();
      foreach (ToolStripButton i in items2) SetSaver(i);

      items2 = null;

      items2 = items.Where(i => i.Text.Contains("Add")).ToArray();
      foreach (ToolStripButton i in items2) SetAdder(i);
      items2 = null;
    }

    public bool CreateEvents(ref DataGridView[] dgvs)
    {
      dGVs = dgvs;

      if (dGVs == null) return false;
      int count = dGVs.Count();
      if (count == 0) return false;

      //  indexes = new HashSet<int>[count];

      // ContextMenuStrip cms = null;

      //for (int j = 0; j < 1; j++)
      for (int j = 0; j < count; j++)
      {
        DataGridView dgv = dGVs[j];

        dgv.KeyUp -= this.KeyUp;
        dgv.Sorted -= (dgv_Sorted);

        dgv.KeyUp += this.KeyUp;
        dgv.Sorted += (dgv_Sorted);

        if (_postpaintRows != null && _shouldPostPaintRow != null)
        {
          dgv.RowPostPaint -= dgv_RowPostPaint;
          dgv.RowPostPaint += dgv_RowPostPaint;
        }
        if (_prepaintRows != null && _shouldPrePaintRow != null)
        {
          dgv.RowPrePaint -= dgv_RowPrePaint;
          dgv.RowPrePaint += dgv_RowPrePaint;
        }
        if (_paintCells != null && _shouldPaintCell != null)
        {
          dgv.CellPainting -= this.dgv_CellPainting;
          dgv.CellPainting += this.dgv_CellPainting;
        }

        dgv.RowValidated -= dgv_RowValidated;
        dgv.RowValidated += dgv_RowValidated;

        dgv.MouseHover -= dgv_MouseHover;
        dgv.MouseHover += dgv_MouseHover;

        dgv.CellEndEdit -= dgv_RowValidated;
        dgv.CellEndEdit += dgv_RowValidated;

        BoldHeaders(ref dgv);
      }

      return true;
    }

    public static void BoldHeaders(ref DataGridView dgv)
    {
      DataTable dt = GetDataSource<DataTable>(ref dgv);

      foreach (DataGridViewColumn c in dgv.Columns)
      {
        DataColumn dtcol = dt.Columns[c.DataPropertyName];

        if (!dtcol.ReadOnly)
        {
          Font actual = c.InheritedStyle.Font;
          c.HeaderCell.Style.Font = new Font(actual, FontStyle.Bold);
        }
      }
    }

    /// <summary>
    /// Method handler for any button that should add a row to the table and then execute the RowAddedMethod
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    public void KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Modifiers != Keys.Control) return;
      Keys k = e.KeyCode;
      result = string.Empty;
      bool doRest = false;
      DataGridView dgv = sender as DataGridView;
      if (usrControl != null)
      {
        UserControl control = (UserControl)this.usrControl;
        control.Validate();
      }

      dgv.EndEdit();

      action = "Nothing done!";

      switch (k)
      {
        #region BASIC

        case Keys.C:
          {
            action = "Copied";
            result = Copy(ref dgv);
            doRest = true;
          }
          break;

        case Keys.A:
          {
            action = "Added";
            result = Add(ref dgv);
            doRest = true;
          }
          break;

        case Keys.T:
          {
            action = "Cloned";
            result = Clone(ref dgv);
            doRest = true;
          }
          break;

        case Keys.V:
          {
            action = "Pasted";
            result = Paste(ref dgv);
            doRest = true;
          }
          break;

        case Keys.Z:
          {
            action = "Undone";
            result = Undo(ref dgv);
            doRest = true;
          }
          break;

        #endregion BASIC

        #region EXPORT SAVE LOAD REFRESH

        case Keys.R:
          {
            action = "Refreshed";
            //refreshes when last bool is null
            ExportLoadSave(ref dgv, false, null);

            doRest = true;
          }
          break;

        case Keys.S:
          {
            action = "Saved";
            ExportLoadSave(ref dgv, false, true);

            doRest = true;
          }
          break;

        case Keys.E:
          {
            SFD.ShowDialog();
            action = "Exported";
            ExportLoadSave(ref dgv, false, false);
            doRest = true;
          }
          break;

        case Keys.L:
          {
            OFD.ShowDialog();
            action = "Loaded";
            ExportLoadSave(ref dgv, true, false);

            doRest = true;
          }
          break;

        #endregion EXPORT SAVE LOAD REFRESH

        #region OTHERS

        case Keys.F:
          {
            action = "Query";
            BindingSource bs = GetDataSource<BindingSource>(ref dgv);
            DataTable dt = GetDataSource<DataTable>(ref dgv);

            search.Link(ref dt, ref bs);

            string column = dt.Columns[0].ColumnName;
            if (dgv.CurrentCell != null)
            {
              column = dgv.Columns[dgv.CurrentCell.ColumnIndex].DataPropertyName;
            }

            search.Field = column;
            search.Search();

            result = "Searching...";

            //keeep this line
            //  bs.Position = search.Result;
          }
          break;

        case Keys.N:
          {
            action = "Applied current Date/Time";
            Type tipo = dgv.CurrentCell.GetType();
            if (tipo.Equals(typeof(CalendarCell)))
            {
              DateTime now = DateTime.Now;
              dgv.CurrentCell.Value = now;
              result = "OK! " + now + " (Now) was applied...";

              doRest = true;
            }
            else result = "This is not a Date/Time cell";
          }
          break;

          #endregion OTHERS
      }

      if (doRest)
      {
        if (_informMethod != null)
        {
          if (!result.Contains("OK")) action = "NOT " + action;

          _informMethod(this.result, this.action);
        }
      }

      SetSaveButtonState(false);
    }

    protected internal void KeyUp(KeyEventArgs arg)
    {
      HashSet<string> hs = null;
      hs = new HashSet<string>();
      Func<DataGridView, bool> selector = o =>
      {
        return hs.Add(GetDataSource<DataTable>(ref o).TableName);
      };

      this.shortdGVs = dGVs.Where(selector).ToList();

      foreach (DataGridView v in shortdGVs) this.KeyUp(v, arg);

      hs.Clear();
      hs = null;
    }

    public delegate void Informer(string Result, string Action);
  }
}