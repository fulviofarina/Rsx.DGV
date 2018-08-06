using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Rsx.DGV
{
  public partial class Control
  {
    protected static string[] CreateDeLinker(ref BindingSource bs)
    {
      //takes the array of DGV provided at the instace creation
      //and finds a databound table from it.
      //then calls a delegate of the Saver2 kind, which should have been provided
      //by the user.
      //The Saver2 kind is a bool (DataTable table) function with saving algorithms from the user.

      string sort = string.Empty;
      string filter = string.Empty;
      if (bs != null)
      {
        sort = bs.Sort;
        filter = bs.Filter;

        bs.EndEdit();
        bs.RaiseListChangedEvents = false;
        bs.SuspendBinding();

        bool suspended = bs.IsBindingSuspended;

        //Rsx.Dumb.BS.DeLinkBS(ref bs);
      }
      return new string[] { filter, sort };
    }

    protected static void CreateLinker(ref BindingSource bs, DataTable dt, string filter, string sort)
    {
      if (bs != null)
      {
        bs.RaiseListChangedEvents = true;
        bs.ResumeBinding();
        bool suspended = bs.IsBindingSuspended;

        //Rsx.Dumb.BS.LinkBS(ref bs, dt, filter, sort);
      }
    }

    protected void ExportLoadSave(ref DataGridView dgv, bool load, bool? save)
    {
      //takes the array of DGV provided at the instace creation
      //and finds a databound table from it.
      //then calls a delegate of the Saver2 kind, which should have been provided
      //by the user.
      //The Saver2 kind is a bool (DataTable table) function with saving algorithms from the user.
      bool exported = !load;
      bool success = false;

      int col = 0;
      int row = 0;

      if (dgv.CurrentCell != null)
      {
        //should go back to selected cell;
        col = dgv.CurrentCell.ColumnIndex;
        row = dgv.CurrentCell.RowIndex;
      }

      BindingSource bs = GetDataSource<BindingSource>(ref dgv);

      try
      {
        DataTable table = GetDataSource<DataTable>(ref bs);
        //this just uses an auxiliar to avoid repetition of saving...
        if (save == null)
        {
          if (_preClick != null) _preClick(null, EventArgs.Empty);
        }

        string[] filtsort = null;
        if (bs != null)
        {
          if (_delinker == null) _delinker = CreateDeLinker;
          filtsort = _delinker(ref bs);
        }

        if (load) //load
        {
          if (!string.IsNullOrWhiteSpace(fileName))
          {
            _loader(fileName);
            fileName = string.Empty;
            success = true;
          }
        }
        else
        {
          //when null refresh
          if (save == null)
          {
            _refreshMethod();
            success = true;
          }
          else if ((bool)save) //save
          {
            IEnumerable<DataRow> rows = table.AsEnumerable();
            _saveMethod2(ref rows);
            success = true;
          }
          else if (!string.IsNullOrWhiteSpace(fileName)) //export
          {
            table.EndLoadData();
            table.WriteXml(fileName, XmlWriteMode.WriteSchema, false);
            table.BeginLoadData();
            fileName = string.Empty;
            success = true;
          }
        }

        if (_linker == null) _linker = CreateLinker;
        _linker(ref bs, table, filtsort[0], filtsort[1]);

        if (save == null)
        {
          if (_postClick != null) _postClick(null, EventArgs.Empty);
        }
      }
      catch (SystemException ex)
      {
        MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
      }

      string meta = "loaded";
      if (exported) meta = "exported";
      else if (save == null) meta = "refreshed";
      else if ((bool)save) meta = "saved";

      if (success) result = "OK! DataTable " + " was " + meta;
      else result = "DataTable " + " was NOT " + meta;

      if (save == null || (bool)save)
      {
        dgv.DataSource = null;
        dgv.DataSource = bs;
        if (dgv.Rows.Count != 0)
        {
          if (col >= 0 && row >= 0) dgv.CurrentCell = dgv[col, row];
        }
      }
    }

        public static string MakeCSVFile(string path, string matrixID, ref DataGridView dgv)
        {
            string file = string.Empty;
            try
            {
                dgv.SelectAll();
            DataObject o = dgv.GetClipboardContent();

           file= makeCSVFile(path, matrixID, ref  o);

                dgv.ClearSelection();
            }
            catch (Exception)
            {


            }
            return file;
        }

        private static string makeCSVFile(string path, string matrixID, ref DataObject o)
        {
            if (o == null) return string.Empty;
            string csv = (string)o.GetData("Csv");

            string file = path + matrixID + ".csv";
            if (File.Exists(file)) File.Delete(file);
            File.WriteAllText(file, csv);
            return file;
        }

        public static string MakeHTMLFile(string path, string matrixID, ref DataGridView dgv, string extension)
        {
            string file = string.Empty;
            try
            {
                dgv.SelectAll();
                DataObject o = dgv.GetClipboardContent();

                file = makeHtmlFile(path, matrixID, ref o, extension);

                dgv.ClearSelection();
            }
            catch (Exception)
            {

               
            }

            return file;
        }

        private static string makeHtmlFile(string path, string matrixID, ref DataObject  o, string extension)
        {
            if (o == null) return string.Empty;
            MemoryStream doc = (MemoryStream)o.GetData("HTML Format");
            byte[] arr = doc.ToArray();
            string file = path + matrixID + extension;
            if (File.Exists(file))         File.Delete(file);
             File.WriteAllBytes(file, arr);
            return file;
        }

        protected void SetSaveButtonState(bool changed)
    {
      if (saveButton != null)
      {
        saveButton.Enabled = changed;
      }
      else if (tsSaveButton != null)
      {
        tsSaveButton.Enabled = changed;
      }
    }

    public void Refresh(object sender, EventArgs e)
    {
      //takes the array of DGV provided at the instace creation
      //and finds a databound table from it.
      //then calls a delegate of the Saver2 kind, which should have been provided
      //by the user.
      //The Saver2 kind is a bool (DataTable table) function with saving algorithms from the user.
      KeyEventArgs args = new KeyEventArgs(Keys.Control | Keys.R);
      KeyUp(args);
      args = null;
    }

    /// <summary>
    /// Creates a delegate of the Saver kind: void (object sender, EventArgs e)
    /// based on a provided delegate of the Saver2 kind: bool (DataTable dt)
    /// when the Saver2 override was used for creating the class.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void Save(object sender, EventArgs e)
    {
      //takes the array of DGV provided at the instace creation
      //and finds a databound table from it.
      //then calls a delegate of the Saver2 kind, which should have been provided
      //by the user.
      //The Saver2 kind is a bool (DataTable table) function with saving algorithms from the user.
      KeyEventArgs arg = new KeyEventArgs(Keys.Control | Keys.S);
      KeyUp(arg);
      arg = null;
    }

    public void AddNewItem(object sender, EventArgs e)
    {
      //The Saver2 kind is a bool (DataTable table) function with saving algorithms from the user.
      KeyEventArgs arg = new KeyEventArgs(Keys.Control | Keys.A);
      KeyUp(arg);
      arg = null;
    }

    private ToolStripButton tsSaveButton;
    private Button saveButton;

    private Linker _linker;
    private DeLinker _delinker;

    private EventHandler _preClick;
    private EventHandler _postClick;

    private Loader _loader;
    private Saver<DataRow> _saveMethod2;
    private Refresher _refreshMethod;
    private AdderEraser _rowAddedMethod;

    public AdderEraser RowAddedMethod
    {
      get { return _rowAddedMethod; }
      set { _rowAddedMethod = value; }
    }
        private AdderEraser _rowDeletedMethod;

        public AdderEraser RowDeletedMethod
        {
            get { return _rowDeletedMethod; }
            set { _rowDeletedMethod = value; }
        }
        public Loader LoadMethod
    {
      get { return _loader; }
      set { _loader = value; }
    }

    public Saver<DataRow> SaveMethod
    {
      get { return _saveMethod2; }
      set { _saveMethod2 = value; }
    }

    public EventHandler PostRefresh
    {
      get { return _postClick; }
      set { _postClick = value; }
    }

    /// <summary>
    /// Assigns a given button the save method for the datasource of interest
    /// </summary>
    /// <param name="button">can be null</param>
    /// <param name="saveMethod">save method</param>
    public EventHandler PreRefresh
    {
      get { return _preClick; }
      set { _preClick = value; }
    }

    public System.Windows.Forms.OpenFileDialog OpenFile
    {
      get { return OFD; }
      set { OFD = value; }
    }

    public System.Windows.Forms.SaveFileDialog SaveFile
    {
      get { return SFD; }
      set { SFD = value; }
    }

    private string fileName;
    private System.Windows.Forms.OpenFileDialog OFD;
    private System.Windows.Forms.SaveFileDialog SFD;

    private void SFD_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
    {
      //    exportPath = SFD.InitialDirectory;
      fileName = SFD.FileName;
    }

    private void OFD_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
    {
      //  exportPath = OFD.InitialDirectory;
      fileName = OFD.FileName;
    }

    public void SetAdder(object button)
    {
      if (button.GetType().Equals(typeof(ToolStripButton)))
      {
        ToolStripButton b = button as ToolStripButton;
        b.Click += this.AddNewItem;
      }
      else if (button.GetType().Equals(typeof(Button)))
      {
        Button b = button as Button;
        b.Click += this.AddNewItem;
      }
    }

    public void SetSaver(object button)
    {
      if (button.GetType().Equals(typeof(ToolStripButton)))
      {
        tsSaveButton = button as ToolStripButton;
        tsSaveButton.Click += this.Save;
      }
      else if (button.GetType().Equals(typeof(Button)))
      {
        saveButton = button as Button;
        saveButton.Click += this.Save;
      }
    }

    public delegate bool Saver<T>(ref IEnumerable<T> rows);

    public delegate void Loader(string file);

    public delegate void Refresher();

    public delegate void AdderEraser(ref DataRow row);

    public delegate void Linker(ref BindingSource bs, DataTable dt, string filter, string sort);

    public delegate string[] DeLinker(ref BindingSource bs);
  }
}