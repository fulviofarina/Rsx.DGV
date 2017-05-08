using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Rsx.Dumb;
namespace Rsx.DGV
{
  public partial class Control
  {
    public static DataGridViewSelectedCellCollection CellsInClipBoard
    {
      get { return Control.currentCol; }
      set { Control.currentCol = value; }
    }

    public static T GetDataSource<T>(ref DataGridView dgv)
    {
      CheckDataSource(ref dgv);
      Type tipo = dgv.DataSource.GetType();
      if (tipo.Equals(typeof(BindingSource)))
      {
        BindingSource bs = dgv.DataSource as BindingSource;
        CheckDataSource(ref bs);
        if (typeof(T).Equals(typeof(DataTable))) return GetDataSource<T>(ref bs);
        else return (T)(bs as object);
      }
      else return (T)dgv.DataSource;
    }

    public static T GetDataSource<T>(ref BindingSource bs)
    {
      object dt = null;
      if (typeof(T).Equals(typeof(DataTable)))
      {
        if (null == bs.DataMember) throw new Exception("BS DataMember is null.  Dumb.GetTable Method");
        if (string.IsNullOrWhiteSpace(bs.DataMember)) throw new Exception("BS DataMember is empty.  Dumb.GetTable Method");
        string tablename = bs.DataMember;
        DataSet set = bs.DataSource as DataSet;
        dt = set.Tables[bs.DataMember];
        return (T)dt;
      }
      else dt = bs.DataSource;

      return (T)dt;
    }

    protected static void CheckDataSource(ref DataGridView dgv)
    {
      if (dgv.DataSource == null) throw new Exception("DataSource is null.  Dumb.GetTable Method");
    }

    protected static void CheckDataSource(ref BindingSource bs)
    {
      if (bs.DataSource == null) throw new Exception("BS DataSource is null.  Dumb.GetTable Method");
    }

    public static string Copy(ref DataGridView dgv)
    {
      if (dgv == null) return "The Data Grid cannot be null";
      currentCol = dgv.SelectedCells;
      return "OK! Selected were copied";
    }

    public string Add(ref DataGridView dgv, bool add = true)
    {
      if (dgv == null) return "The Data Grid cannot be null";

      string indexfield = string.Empty;
      DataTable table = GetDataSource<DataTable>(ref dgv);
      DataColumn col = table.PrimaryKey.FirstOrDefault();
      if (col != null) indexfield = col.ColumnName;

      string aux = "added";
            if (add)
            {
                DataRow s = table.NewRow();
                table.Rows.Add(s);
              
                    _rowAddedMethod?.Invoke(ref s);
               
            }
            else
            {
                aux = "deleted";
                if (dgv.CurrentRow != null)
                {
                    DataRow s = (dgv.CurrentRow.DataBoundItem as DataRowView).Row;
                    s.Delete();
                    //  DataRow s = null;
                    _rowDeletedMethod?.Invoke(ref s);
                }
            }
      /*
        Func<DataRowView, bool> finder = f =>
        {
           DataRow s2 = f.Row;
           return s.Field<object>(indexfield).Equals(s2.Field<object>(indexfield));
        };
       */

    //  if (!indexfield.Equals(string.Empty))
      //{
      //  BindingSource BS = GetDataSource<BindingSource>(ref dgv);
      //  BS.Sort = indexfield + " desc";

       // BS.RemoveFilter(); ///?????
      //}

      return "OK! A new Item was " + aux;
    }

    public static string Paste(ref DataGridView dgv)
    {
      if (currentCol == null) return "Select something to copy first";

      int i = 0;
      int count = currentCol.Count;
      bool ronly = false;

      foreach (DataGridViewCell cell in dgv.SelectedCells)
      {
        if (dgv.Columns[cell.ColumnIndex].ReadOnly)
        {
          if (!ronly) ronly = true;
          continue;
        }
        DataGridViewCell c = currentCol[i];
        cell.Value = c.Value;
        if (i < count - 1) i++;
      }

      if (ronly) return ("Some destination cells are read-only, nothing pasted there");
      else return "OK! Selected cells were pasted with clipboard content";
    }

    public static string Clone(ref DataGridView dgv)
    {
      if (dgv == null) return "The Data Grid cannot be null";

      DataTable dt = GetDataSource<DataTable>(ref dgv);

      Tables.CloneRows(ref dgv, ref dt);

      return "OK! Selected cells were cloned";
    }

    public static string Undo(ref DataGridView dgv)
    {
      if (dgv == null) return "The Data Grid cannot be null";
      DataTable dt = GetDataSource<DataTable>(ref dgv);

      dt.RejectChanges();

      return "OK! changes were rejected";
    }

    private static DataGridViewSelectedCellCollection currentCol;
  }
}