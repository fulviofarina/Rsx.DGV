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
            ICalculableRow Icalc = GetICalculable();
            if (Icalc != null)
            {
                Icalc.ToDo = !Icalc.ToDo;
                base.DataGridView.NotifyCurrentCellDirty(true);
            }
            base.OnMouseDoubleClick(e);
            base.DataGridView.ClearSelection();
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
            //changed is not used
            //but could be used
            ICalculableRow Icalc = GetICalculable();

            bool? todo = Icalc?.ToDo;
            bool? isBusy = Icalc?.IsBusy;
            Color colr = getColor(todo, isBusy);
            bool changed = hasChanged(ref cellStyle, ref colr);

            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }

        /// <summary>
        /// Gets the ICalculableRow Interface to the Row that this DGV cell is binded to
        /// </summary>
        /// <returns></returns>
        public ICalculableRow GetICalculable()
        {
            ICalculableRow Icalc = null;
            DataRow u = ((this.OwningRow?.DataBoundItem as DataRowView)?.Row) as DataRow;
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
            Color colr = Color.DarkGreen;
            if (todo != null && todo == true)
            {
                if (isBusy != null)
                {
                    if (isBusy == true) colr = Color.DarkOrange;
                    else colr = Color.DarkRed;
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

            Color defaultColor = Color.Transparent;
            Color defaultColor2 = Color.Transparent;


            Style.SelectionBackColor = defaultColor2;
            Style.SelectionForeColor = defaultColor2;

       
            Style.BackColor = defaultColor;
            Style.ForeColor = defaultColor;
        }
    }
}