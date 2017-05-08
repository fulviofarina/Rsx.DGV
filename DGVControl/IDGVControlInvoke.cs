using System;
using System.Data;
using System.Windows.Forms;

namespace Rsx.DGV
{
    public interface IDGVControlInvoke
    {
        //DataGridView[] DGVs { get; set; }
       // Control.Informer InformMethod { get; set; }
        string LastAction { get; }
        string LastResult { get; }
      //  Control.Loader LoadMethod { get; set; }
        OpenFileDialog OpenFile { get; set; }
      //  DataGridViewCellPaintingEventHandler PaintCellsMethod { get; set; }
      //  DataGridViewRowPostPaintEventHandler PostPaintRowsMethod { get; set; }
      //  EventHandler PostRefresh { get; set; }
      //  DataGridViewRowPrePaintEventHandler PrePaintRowsMethod { get; set; }
      //  EventHandler PreRefresh { get; set; }
     //   Control.AdderEraser RowAddedMethod { get; set; }
     //   Control.AdderEraser RowDeletedMethod { get; set; }
        SaveFileDialog SaveFile { get; set; }
       // Control.Saver<DataRow> SaveMethod { get; set; }
       // Control.CellPaintChecker ShouldPaintCellMethod { get; set; }
       // Control.RowPostPaintChecker ShouldPostPaintRowMethod { get; set; }
       // Control.RowPrePaintChecker ShouldPrePaintRowMethod { get; set; }
        object UsrControl { get; set; }

        string Add(ref DataGridView dgv, bool add = true);
        void AddNewItem(object sender, EventArgs e);
      //  void CreateButtonEvents(ref ToolStripButton[] items);
       // bool CreateDGVEvents();
      //  bool CreateDGVEvents(ref DataGridView[] dgvs);
        void KeyUp(object sender, KeyEventArgs e);
        void Refresh(object sender, EventArgs e);
        void Save(object sender, EventArgs e);
      //  void SetAdder(object button);
       // void SetContext(string controlHeader, ref DataGridView[] dgvs, ContextMenuStrip cms);
      //  void SetSaver(object button);
    }
}