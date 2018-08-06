using Rsx.Dumb;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Rsx.DGV
{
    /// <summary>
    /// The kind of column that shows wether the system is Busy calculating or a calculation needs to
    /// be performed based on the red-green and orange scheme in traffic lights
    /// </summary>
    public class CalculableColumn : DataGridViewTextBoxColumn
    {
        public CalculableColumn()
            : base()
        {
            CellTemplate = new CalculableCell();
        }
    }

    /// <summary>
    /// The kind of cell that shows wether the system is Busy calculating or a calculation needs to
    /// be performed based on the red-green and orange scheme in traffic lights
    /// </summary>
    public class CalculableCell : DataGridViewTextBoxCell
    {
        protected override void OnMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
          //  if (this.OwningRow.Index < 0) return;

            ICalculableRow Icalc = GetICalculable(e.RowIndex);
            if (Icalc != null)
            {
                Icalc.ToDo = !Icalc.ToDo;
            
            }
           
             base.DataGridView.NotifyCurrentCellDirty(true);
             base.DataGridView.ClearSelection();
              base.OnMouseDoubleClick(e);

        }

        /// <summary>
        /// Paints red, green and orange based on the ToDo and IsBusy conditionals
        /// </summary>
        /// <param name="graphics">           </param>
        /// <param name="clipBounds">         </param>
        /// <param name="cellBounds">         </param>
        /// <param name="rowIndex">           </param>
        /// <param name="cellState">          </param>
        /// <param name="value">              </param>
        /// <param name="formattedValue">     </param>
        /// <param name="errorText">          </param>
        /// <param name="cellStyle">          </param>
        /// <param name="advancedBorderStyle"></param>
        /// <param name="paintParts">         </param>
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {

            setDefaultCellStyle();

            //if (this.OwningRow.Index < 0) return;

            //changed is not used
            //but could be used
            ICalculableRow Icalc = GetICalculable(rowIndex);

            bool? todo = Icalc?.ToDo;
            bool? isBusy = Icalc?.IsBusy;
            Color colr = getColor(todo, isBusy);
             bool changed = hasChanged(ref cellStyle, ref colr);

        

            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

        //    base.DataGridView.NotifyCurrentCellDirty(true);
          //  base.DataGridView.ClearSelection();
        }

        /// <summary>
        /// Gets the ICalculableRow Interface to the Row that this DGV cell is binded to
        /// </summary>
        /// <returns></returns>
        public ICalculableRow GetICalculable(int rowIndex)
        {
            ICalculableRow Icalc = null;
            if (rowIndex <= -1) return Icalc;
            DataGridViewRow r = this.DataGridView.Rows[rowIndex];
            DataRowView v = r?.DataBoundItem as DataRowView;
            DataRow u = (v?.Row) as DataRow;
            if (!EC.IsNuDelDetch(u))
            {
                Icalc = u as ICalculableRow;
            }
            return Icalc;
        }

        /// <summary>
        /// after color change this return wether a change was performed
        /// </summary>
        /// <param name="cellStyle"></param>
        /// <param name="colr">     </param>
        /// <returns></returns>
        private static bool hasChanged(ref DataGridViewCellStyle cellStyle, ref Color colr)
        {
            bool changed = false;
            if (colr != cellStyle.BackColor)
            {
                cellStyle.BackColor = colr;
                cellStyle.ForeColor = colr;
                changed = true;
            }
            return changed;
        }

        /// <summary>
        /// gets the color to fill the cell
        /// </summary>
        /// <param name="todo">  </param>
        /// <param name="isBusy"></param>
        /// <returns></returns>
        private static Color getColor(bool? todo, bool? isBusy)
        {
            Color colr = Color.PaleGreen;
            if (todo != null && todo == true)
            {
                if (isBusy != null)
                {
                    if (isBusy == true) colr = Color.Orange;
                    else colr = Color.Coral;
                }
            }

            return colr;
        }

        /// <summary>
        /// The kind of cell that shows wether the system is Busy calculating or a calculation needs
        /// to be performed based on the red-green and orange scheme in traffic lights
        /// </summary>
        public CalculableCell() : base()
        {

            setDefaultCellStyle();
        }

        private void setDefaultCellStyle()
        {
            Color defaultColor = Color.Transparent;
            Color defaultColor2 = Color.Transparent;


            Style.SelectionBackColor = defaultColor;
            Style.SelectionForeColor = defaultColor2;


            Style.BackColor = defaultColor;
            Style.ForeColor = defaultColor;
        }
    }
}