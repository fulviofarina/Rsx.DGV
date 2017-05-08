using System;
using System.Data;
using System.Windows.Forms;

namespace Rsx.DGV
{
    public interface IDGVControlMethods
    {
        void SetSaver(object button);
        void SetAdder(object button);
        //  DataGridView[] DGVs { get; set; }
        Control.Informer InformMethod { get; set; }
      //  string LastAction { get; }
      //  string LastResult { get; }
        Control.Loader LoadMethod { get; set; }
    //    OpenFileDialog OpenFile { get; set; }
        DataGridViewCellPaintingEventHandler PaintCellsMethod { get; set; }
        DataGridViewRowPostPaintEventHandler PostPaintRowsMethod { get; set; }
        EventHandler PostRefresh { get; set; }
        DataGridViewRowPrePaintEventHandler PrePaintRowsMethod { get; set; }
        EventHandler PreRefresh { get; set; }
        Control.AdderEraser RowAddedMethod { get; set; }
        Control.AdderEraser RowDeletedMethod { get; set; }
    //    SaveFileDialog SaveFile { get; set; }
        Control.Saver<DataRow> SaveMethod { get; set; }
        Control.CellPaintChecker ShouldPaintCellMethod { get; set; }
        Control.RowPostPaintChecker ShouldPostPaintRowMethod { get; set; }
        Control.RowPrePaintChecker ShouldPrePaintRowMethod { get; set; }
        object UsrControl { get; set; }

       
    }
}