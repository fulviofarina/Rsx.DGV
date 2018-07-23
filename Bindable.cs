using Rsx.Dumb;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Rsx.DGV
{


    public interface IBindableDGVColumn
    {
        DefaultAction DefaultAction { set; }

        string BindingPreferenceField { get; set; }
        DataRow BindingPreferenceRow { get; set; }

        void SetAction();
    }

    public enum DefaultAction
    {
        ReadOnly,
        Visibility,
        Enable
    };
    public partial class BindableDGVCell : DataGridViewTextBoxCell
    {

        protected internal BindableDGVColumn parent
        {
            get
            {
                return this.OwningColumn as BindableDGVColumn;
            }
        }

        /// <summary>
        /// Sets the Style
        /// </summary>
        /// <param name="readonlyMode"></param>
        protected internal void setCellReadOnlyOrNormal()
        {
            if (parent.ReadOnly)
            {
                Style.BackColor = parent.arrayOfBackColors[0];
                this.Style.ForeColor = parent.arrayOfForeColors[0];
                this.Style.SelectionBackColor = parent.arrayOfBackColors[0];
                this.Style.SelectionForeColor = parent.arrayOfForeColors[0];
            }
            else
            {
                Style.BackColor = parent.arrayOfBackColors[2];
                Style.ForeColor = parent.arrayOfForeColors[2];
                Style.SelectionBackColor = parent.arrayOfBackColors[1];
                Style.SelectionForeColor = parent.arrayOfForeColors[1];
            }

        }

        /// <summary>
        /// Sets if should be read only or not
        /// </summary>
        /*
        protected internal void setReadOnlyOrNot()
        {
            if (!string.IsNullOrEmpty(parent?.BindingPreferenceField))
            {
                bool Isreadonly = false;
                Isreadonly = (bool)parent?.BindingPreferenceRow[parent?.BindingPreferenceField];
                setCellReadOnlyOrNormal(Isreadonly);
                if (Isreadonly != this.OwningColumn.ReadOnly)
                {
                    this.OwningColumn.ReadOnly = Isreadonly;
                }
            }
        }
        */
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {

            parent.SetAction();

            if (parent.DefaultAction == DefaultAction.ReadOnly)
            {
                //   parent.SetAction();
                setCellReadOnlyOrNormal();
            }

            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }

        public BindableDGVCell() : base()
        {
            Style.Font = new Font("Segoe UI", 14.25f, FontStyle.Bold);
        }
    }

    public class BindableDGVColumn : DataGridViewTextBoxColumn, IBindableDGVColumn
    {
        //default colors
        public Color[] arrayOfBackColors = new Color[] { Color.FromArgb(64, 64, 64), Color.White, Color.White };
        public Color[] arrayOfForeColors = new Color[] { Color.Yellow, Color.Black, Color.Black };

        public BindableDGVColumn() : base()
        {

        }

        protected internal DefaultAction defaultAction = DefaultAction.ReadOnly;

        protected internal string bindingField = string.Empty;

        protected internal DataRow bindingRow = null;

        public string BindingPreferenceField
        {
            get
            {
                return bindingField;
            }

            set
            {
                bindingField = value;
            }
        }

        public DataRow BindingPreferenceRow
        {
            get
            {
                return bindingRow;
            }

            set
            {
                bindingRow = value;

                SetAction();

            }
        }

        public void SetAction()
        {

            if (defaultAction == DefaultAction.ReadOnly)
            {
                SetReadOnly();
            }
            else if (defaultAction == DefaultAction.Visibility)
            {
                SetVisibility();
            }
            else if (defaultAction == DefaultAction.Enable)
            {
                SetEnable();
            }
            //   SetRounding();

        }

        public void SetEnable()
        {
            if (string.IsNullOrEmpty(bindingField)) return;
            bool? Isreadonly = (bool?)bindingRow?[bindingField];
            if (Isreadonly != null) this.ReadOnly = (bool)!Isreadonly;

        }
        protected internal char[] formatchars = { 'G', 'F', 'N', 'E', 'C' };
        protected internal string bindingRoundingField = string.Empty;
        public void SetRounding()
        {

            try
            {
                if (!string.IsNullOrEmpty(bindingRoundingField))
                {
                    string rounding = (string)bindingRow?.Field<string>(bindingRoundingField);

                    if (rounding.Count() == 2)
                    {
                        char first = rounding.ToUpper()[0];
        
                        if (formatchars.Contains(first))
                        {
                            char second = rounding[1];
                            if (char.IsNumber(second))
                            {
                                this.DefaultCellStyle.Format = rounding;
                            }
                        }
                    }
                    this.DataGridView.InvalidateColumn(this.Index);
                }
            }
            catch (FormatException)
            {

            }

        }

        public string BindingRoundingField
        {
            get
            {
                return bindingRoundingField;
            }
            set
            {
                bindingRoundingField = value;
            }
        }

        public DefaultAction DefaultAction
        {

            get
            {
                return defaultAction;
            }
            set
            {
                defaultAction = value;
            }
        }

        public void SetReadOnly()
        {
            if (string.IsNullOrEmpty(bindingField)) return;
            bool? Isreadonly = (bool?)bindingRow?[bindingField];
            if (Isreadonly != null) this.ReadOnly = (bool)Isreadonly;

        }
        public void SetVisibility()
        {
            if (string.IsNullOrEmpty(bindingField)) return;
            bool? isvisible = (bool?)bindingRow?[bindingField];
            if (isvisible != null) this.Visible = (bool)isvisible;


        }
    }







}



