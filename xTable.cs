using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Rsx.DGV
{
    public class xTable
    {
        public static Comparison<string> stringsorter = delegate (string a, string b)
        {
            return a.Remove(0, a.Length - 1).CompareTo(b.Remove(0, b.Length - 1));
        };

        /// <summary>
        /// Returns a new DataTable from a given array of rows, taking into account the rows's table structure when found.
        /// </summary>
        /// <param name="rows">The array of rows to populate the new DataTable</param>
        /// <returns></returns>
        public static DataTable TableFrom(ref DataRow[] rows)
        {
            DataTable table = null;
            if (rows != null && rows.Length != 0)
            {
                table = new DataTable();
                if (rows.Length != 0) foreach (DataRow r in rows) table.ImportRow(r);
                // DataTable fromtable = rows[0].Table;
                //if (fromtable != null)   table = fromtable.Clone();
            }
            // else table = new DataTable();

            return table;
        }

        /// <summary>
        /// Returns a new DataTable with the calculated weighted average, weighted SD and observed SD from any DataTable
        /// </summary>
        /// <param name="toFilterAndAvg">DataTable to calculate values from</param>
        /// <param name="filterFields">Array of Fields (ColumnNames) to filter the input DataTable before the calculations takes place</param>
        /// <param name="filterFieldsValues">Array of values linked to each one of the Fields to use as filters</param>
        /// <param name="filterFieldsAndOr">Array of operators: AND, OR, LIKE, etc. This array should have a Length = (Fields[].Lenght -1)</param>
        /// <param name="filterFieldsEqualities">Array of equalities to use in the filters: =, >=, etc </param>
        /// <param name="fieldToAvg">The Field (ColumnName) to compute the averages from</param>
        /// <param name="fieldSD">The Field (ColumnName) to compute the weights from</param>
        /// <param name="iterateLastfilterField">In case the computation should be performed through iteration on the values of the last Field in the array to use as filter</param>
        /// <returns>A New DataTable containing the Fields and values filtered and the computed values</returns>
        public static DataTable CalculateWeightAvgAndObsSD(ref DataTable toFilterAndAvg, string[] filterFields, string[] filterFieldsValues, string[] filterFieldsAndOr, string[] filterFieldsEqualities, string fieldToAvg, string fieldSD, bool iterateLastfilterField)
        {
            DataTable averaged = null;

            if (toFilterAndAvg != null)
            {
                if (toFilterAndAvg.Rows.Count != 0)
                {
                    if (iterateLastfilterField)
                    {
                        averaged = new DataTable();
                        string fielditerator = filterFields.Last();
                        int indexOflast = (filterFields.Length - 1);
                        ICollection<string> fielditeratorValues = Dumb.HashFrom<string>(toFilterAndAvg.Columns[fielditerator]);
                        DataTable AvgOneIteration = null;
                        foreach (string r in fielditeratorValues)
                        {
                            filterFieldsValues[indexOflast] = r;
                            AvgOneIteration = xTable.CalculateWeightAvgAndObsSD(ref toFilterAndAvg, filterFields, filterFieldsValues, filterFieldsAndOr, filterFieldsEqualities, fieldToAvg, fieldSD);
                            averaged.Load(AvgOneIteration.CreateDataReader());
                        }
                    }
                    else averaged = xTable.CalculateWeightAvgAndObsSD(ref toFilterAndAvg, filterFields, filterFieldsValues, filterFieldsAndOr, filterFieldsEqualities, fieldToAvg, fieldSD);
                    if (averaged != null) averaged.AcceptChanges();
                }
            }

            if (averaged == null) averaged = new DataTable();

            return averaged;
        }

        /// <summary>
        /// Returns a new DataTable with the calculated weighted average, weighted SD and observed SD from any DataTable
        /// </summary>
        /// <param name="toFilterAndAvg">DataTable to calculate values from</param>
        /// <param name="filterFields">Array of Fields (ColumnNames) to filter the input DataTable before the calculations takes place</param>
        /// <param name="filterFieldsValues">Array of values linked to each one of the Fields to use as filters</param>
        /// <param name="filterFieldsAndOr">Array of operators: AND, OR, LIKE, etc. This array should have a Length = (Fields[].Lenght -1)</param>
        /// <param name="filterFieldsEqualities">Array of equalities to use in the filters: =, >=, etc </param>
        /// <param name="fieldToAvg">The Field (ColumnName) to compute the averages from</param>
        /// <param name="fieldSD">The Field (ColumnName) to compute the weights from</param>
        /// <returns>A New DataTable containing the Fields and values filtered and the computed values</returns>
        public static DataTable CalculateWeightAvgAndObsSD(ref DataTable toFilterAndAvg, string[] filterFields, string[] filterFieldsValues, string[] filterFieldsAndOr, string[] filterFieldsEqualities, string fieldToAvg, string fieldSD)
        {
            DataTable table = null;

            if (toFilterAndAvg != null)
            {
                if (toFilterAndAvg.Rows.Count != 0)
                {
                    table = new DataTable();
                    DataView v = toFilterAndAvg.AsDataView();
                    String filter = String.Empty;
                    for (int j = 0; j < filterFields.Length; j++)
                    {
                        filter += filterFields[j] + filterFieldsEqualities[j] + "'" + filterFieldsValues[j] + "'";
                        if (j < filterFields.Length - 1) filter += " " + filterFieldsAndOr[j] + " ";
                    }
                    v.RowFilter = filter;
                    v.RowStateFilter = DataViewRowState.CurrentRows;

                    if (v.Count != 0)
                    {
                        DataTable toAvg = v.ToTable();

                        for (int j = 0; j < filterFields.Length; j++)
                        {
                            table.Columns.Add(filterFields[j], typeof(string));
                        }

                        table.Columns.Add(fieldToAvg, typeof(double));
                        table.Columns.Add("SD", typeof(double));
                        table.Columns.Add("ObsSD", typeof(double));
                        table.Columns.Add("n", typeof(int));

                        double weight_i = 0.0;
                        double weight2 = 0.0;
                        double avg = 0.0;
                        double weight = 0.0;
                        double sd = 0.0;
                        double sd_biased = 0.0;
                        int n = 0;
                        double avgi = 0.0;
                        double sdi = 0.0;
                        double diffi = 0.0;

                        foreach (DataRow row in toAvg.Rows)
                        {
                            if (row.RowState != DataRowState.Deleted)
                            {
                                weight_i = 1;
                                avgi = 0;

                                try
                                {
                                    weight_i = (double)row[fieldSD];
                                    weight_i = Math.Pow(weight_i, -2.0);
                                    avgi = (double)row[fieldToAvg] * weight_i;
                                }
                                catch (Exception e)
                                {
                                }
                                avg += avgi;
                                weight += weight_i;
                                n++;
                            }
                        }
                        if (n >= 1)
                        {
                            avg /= weight;
                            sd = 0.0;
                            sd_biased = 0;

                            if (n > 1)
                            {
                                weight = 0.0;
                                weight2 = 0;

                                foreach (DataRow row2 in toAvg.Rows)
                                {
                                    if (row2.RowState != DataRowState.Deleted)
                                    {
                                        weight_i = 1;
                                        sdi = 0;
                                        diffi = 0;

                                        try
                                        {
                                            weight_i = (double)row2[fieldSD];
                                            weight_i = Math.Pow(weight_i, -2.0);
                                            diffi = (double)row2[fieldToAvg] - avg;
                                            sdi = diffi * diffi * weight_i;
                                        }
                                        catch (Exception e)
                                        {
                                        }
                                        sd += sdi;
                                        weight += weight_i;
                                        weight2 += Math.Pow(weight_i, 2.0);
                                    }
                                }

                                sd_biased = (sd / weight);
                                sd_biased = (Math.Sqrt(sd_biased) * 100) / avg;

                                sd = (sd * weight) / (Math.Pow(weight, 2.0) - weight2);
                                sd = (Math.Sqrt(sd) * 100.0) / avg;
                            }
                            else sd = Math.Sqrt(1.0 / weight);

                            //create the data row with averages
                            DataRow g = table.NewRow();
                            if (double.IsNaN(avg) || avg == 0.0)
                            {
                                avg = 0.0;
                                g.SetColumnError(fieldToAvg, "Average is Null or NaN");
                            }
                            if (double.IsNaN(sd))
                            {
                                sd = 0.0;
                                g.SetColumnError("SD", "StandardDeviation is Null or NaN");
                            }

                            for (int j = 0; j < filterFields.Length; j++)
                            {
                                g.SetField<string>(filterFields[j], filterFieldsValues[j].ToString());
                            }
                            g.SetField<double>(fieldToAvg, avg);
                            g.SetField<double>("SD", sd);
                            g.SetField<double>("ObsSD", sd_biased);
                            g.SetField<int>("n", n);

                            table.LoadDataRow(g.ItemArray, LoadOption.OverwriteChanges);

                            table.AcceptChanges();
                        }
                    }
                }
            }

            return table;
        }

        public static System.Data.DataTable DGVToTable(ref System.Windows.Forms.DataGridView dgv)
        {
            System.Data.DataTable table = new System.Data.DataTable();

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                table.Columns.Add(col.HeaderText, typeof(object), String.Empty);
            }

            System.Collections.ArrayList list = new System.Collections.ArrayList();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    list.Add(cell.Value);
                }
                table.LoadDataRow(list.ToArray(), true);
                list.Clear();
            }

            return table;
        }

        public static void Clean(ref DataGridView DGV)
        {
            DGV.DataSource = null;
            DGV.Columns.Clear();
            DGV.Tag = null;
            DGV.AutoGenerateColumns = true;
        }

        public static void Fill_Xij(ref DataGridView DGV, int startColumn, string FieldinCells, string FieldtoTipCells, int TipDecimalDigits)
        {
            IEnumerable<DataGridViewCell> cells = Selected(DGV, true);
            cells = cells.Where(CellsFromCol(startColumn));

            foreach (DataGridViewCell cell in cells)
            {
                DataRow tag = (DataRow)cell.Tag;
                cell.Value = tag[FieldinCells];
                if (FieldtoTipCells.Equals(string.Empty)) continue;
                if (tag.IsNull(FieldtoTipCells)) continue;
                cell.ToolTipText = Decimal.Round(Convert.ToDecimal(tag[FieldtoTipCells]), TipDecimalDigits).ToString();
            }

            DGV.ClearSelection();
        }

        public static void CleanCells(ref DataGridView DGV, int fromColumn)
        {
            DGV.SuspendLayout();

            DGV.SelectAll();
            Func<DataGridViewCell, bool> tokill = c =>
                {
                    return (c.ColumnIndex >= fromColumn && c.Tag == null);
                };

            IEnumerable<DataGridViewCell> cells = DGV.SelectedCells.Cast<DataGridViewCell>().Where(tokill);

            foreach (DataGridViewCell cell in cells)
            {
                cell.ErrorText = null;
                cell.Style.BackColor = Color.White;
                cell.Style.SelectionBackColor = Color.White;
                cell.ToolTipText = null;
                cell.Value = null;
            }

            cells = null;

            IEnumerable<DataGridViewRow> del = DGV.SelectedRows.Cast<DataGridViewRow>().ToList();

            foreach (DataGridViewRow r in del)
            {
                cells = r.Cells.Cast<DataGridViewCell>().Where(tokill);
                if (cells.Count() == r.Cells.Count - fromColumn) DGV.Rows.Remove(r);
                cells = null;
            }

            del = null;

            DGV.ClearSelection();

            DGV.ResumeLayout();
        }

        private static Func<DataGridViewCell, bool> GoodCell = o =>
            {
                if (o.Tag == null) return false;
                if (o.Tag.GetType().Equals(typeof(List<DataRow>))) return false;
                else
                {
                    DataRow tag = null;
                    tag = (DataRow)o.Tag;
                    if (!Dumb.IsNuDelDetch(tag)) return true;
                    else return false;
                }

                /*
                  if (o.Tag.GetType().Equals(typeof(List<DataRow>)))
                  {
                      IEnumerable<DataRow> enums =  (o.Tag as IEnumerable<DataRow>);
                      int i = 0;
                      tag = enums.ElementAt(i);
                      while (Dumb.IsNuDelDetch(tag))
                      {
                          i++;
                          if (i < enums.Count())
                          {
                              tag = enums.ElementAt(i);
                          }
                      }
                  }
              else
             */
            };

        /*
       public static Func< DataGridViewRow, IEnumerable<DataGridViewCell>> GoodCellSelector = (o) =>
       {
           IEnumerable<DataGridViewCell> cells = o.Cells.OfType<DataGridViewCell>();
           cells = cells.Where(GoodCell).ToList();
           return cells;
       };
         */

        public static Func<int, Func<DataGridViewCell, bool>> CellsByCol = i =>
        {
            Func<DataGridViewCell, bool> cellbyCol = c =>
     {
         if (c.ColumnIndex == i && GoodCell(c)) return true;
         else return false;
     };

            return cellbyCol;
        };

        public static Func<int, Func<DataGridViewCell, bool>> CellsFromCol = i =>
        {
            Func<DataGridViewCell, bool> cellbyCol = c =>
            {
                if (c.ColumnIndex >= i && GoodCell(c)) return true;
                else return false;
            };

            return cellbyCol;
        };

        public static IEnumerable<DataGridViewCell> Selected(DataGridView dataGridView, bool all)
        {
            if (all)
            {
                dataGridView.SelectAll();
            }
            return dataGridView.SelectedCells.OfType<DataGridViewCell>();
        }

        public static Func<DataGridView, int, int, bool> GoodColRowCell = (dgv, col, row) =>
        {
            bool good = false;
            if (col >= 0 && col <= dgv.ColumnCount)
            {
                if (row >= 0 && row <= dgv.RowCount)
                {
                    DataGridViewCell e = dgv[col, row];
                    good = GoodCell(e);
                }
            }
            return good;
        };

        public static Func<DataGridView, bool> GoodDGV = dgv =>
        {
            if (dgv != null && dgv.Rows.Count != 0) return true;
            return false;
        };

        public static void Fill_Zij(ref DataGridViewColumn DGVcolumn, string FieldinCells, string FieldtoTipCells)
        {
            DataGridView dataGridView = DGVcolumn.DataGridView;
            DGVcolumn.HeaderText = FieldinCells;

            IEnumerable<DataGridViewCell> cells = Selected(dataGridView, true);
            cells = cells.Where(CellsByCol(DGVcolumn.Index)).ToList();

            bool emptyFieldToTip = FieldtoTipCells.Equals(string.Empty);

            foreach (DataGridViewCell cell in cells)
            {
                DataRow tag = (DataRow)cell.Tag;
                if (tag.IsNull(FieldinCells)) continue;
                cell.Value = tag[FieldinCells];
                if (emptyFieldToTip) continue;
                if (tag.IsNull(FieldtoTipCells)) continue;
                cell.ToolTipText = tag[FieldtoTipCells].ToString();
            }

            dataGridView.ClearSelection();
        }

        /// <summary>
        /// the best method so far... Linq
        /// </summary>
        /// <param name="view"></param>
        /// <param name="fieldX"></param>
        /// <param name="Zfields"></param>
        /// <param name="dgv"></param>
        public static void New(ref DataView view, String fieldX, String fieldXSort, String[] Zfields, ref DataGridView dgv, int MinGreen)
        {
            Clean(ref dgv);

            int maxVal = 0xff;

            if (view.Count == 0) return;

            dgv.Tag = view;

            dgv.ReadOnly = true;
            dgv.Cursor = Cursors.Hand;
            dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToOrderColumns = true;

            foreach (string AZfield in Zfields)
            {
                if (!dgv.Columns.Contains(AZfield)) dgv.Columns.Add(AZfield, AZfield);
            }

            Func<DataRowView, string> keySel = o =>
            {
                string keyLong = string.Empty;
                foreach (string AZfield in Zfields) keyLong += o[AZfield];

                return keyLong;
            };

            IEnumerable<DataRowView> enumerable = view.Cast<DataRowView>().ToList();
            IEnumerable<IGrouping<string, DataRowView>> EnergyGroup = enumerable.GroupBy(keySel).ToList();

            Random random = new Random();
            int red = 0;
            int blue = 0;
            HashSet<object> set = new HashSet<object>();

            int w = 0;
            foreach (IGrouping<string, DataRowView> i in EnergyGroup)
            {
                IEnumerable<DataRow> rows = i.Select(o => o.Row).ToList();
                w = dgv.Rows.Add();

                if (set.Add(i.Key))
                {
                    red = random.Next(MinGreen, maxVal);
                    blue = random.Next(MinGreen, maxVal);
                }

                for (int a = 0; a < Zfields.Count(); a++)
                {
                    string AZfield = Zfields[a];
                    object key = rows.First()[AZfield];
                    dgv[AZfield, w].Value = key;
                    dgv[AZfield, w].Tag = rows;
                    dgv[AZfield, w].Style.BackColor = Color.FromArgb(red, MinGreen, blue);
                    dgv[AZfield, w].Style.ForeColor = Color.White;
                }

                //fill rows with x columns
                if (string.IsNullOrEmpty(fieldXSort))
                {
                    rows = rows.OrderByDescending(o => o.Field<double>(fieldXSort));
                }

                foreach (DataRow r in rows)
                {
                    string x = r[fieldX].ToString().ToUpper();
                    if (!dgv.Columns.Contains(x)) dgv.Columns.Add(x, x);
                    DataGridViewCell cell1 = dgv[x, w];
                    if (cell1.Tag == null)
                    {
                        cell1.ErrorText = string.Empty;
                        r.RowError = string.Empty;
                        cell1.Tag = r;
                    }
                    else
                    {
                        DataRow tag = (DataRow)cell1.Tag;
                        cell1.ErrorText += "Duplicated by Row: " + r.Table.Rows.IndexOf(r).ToString() + "\n";
                        tag.RowError += cell1.ErrorText + "\n";
                        r.RowError += "Duplicating Row: " + tag.Table.Rows.IndexOf(tag).ToString() + "\n";
                    }
                }
            }
        }

        #region old?

        public static void New(int ZColumns, ref DataColumn X, ref DataColumn Y, ref DataGridView dgv)
        {
            Clean(ref dgv);

            if (X.Table.Rows.Count != 0)
            {
                dgv.Tag = X.Table;
                int num = 1;
                while (num <= ZColumns)
                {
                    dgv.Columns.Add(null, null);
                    num++;
                }
                dgv.Columns.Add(Y.ColumnName, Y.ColumnName);
                List<string> list = new List<string>(Dumb.HashFrom<string>(X));
                list.Sort(stringsorter);
                foreach (string xi in list)
                {
                    dgv.Columns.Add(xi, xi);
                }
                List<string> list2 = new List<string>(Dumb.HashFrom<string>(Y));
                int w = 0;
                ArrayList rowcol = new ArrayList();

                foreach (string yi in list2)
                {
                    w = dgv.Rows.Add();
                    rowcol.Clear();

                    foreach (DataRow row in Y.Table.Rows)
                    {
                        if (row.RowState != DataRowState.Deleted)
                        {
                            if (yi.CompareTo(row[Y.ColumnName].ToString()) == 0)
                            {
                                rowcol.Add(row); //add toRow the collection of row in a column

                                if (dgv[row[X.ColumnName].ToString(), w].Tag == null)
                                {
                                    dgv[row[X.ColumnName].ToString(), w].Tag = row;
                                    dgv[Y.ColumnName, w].Value = yi;
                                    dgv[Y.ColumnName, w].Tag = row;
                                    for (num = 0; num <= ZColumns; num++)
                                    {
                                        dgv[num, w].Tag = row;
                                    }
                                }
                                else
                                {
                                    DataGridViewCell cell1 = dgv[row[X].ToString(), w];
                                    cell1.ErrorText = cell1.ErrorText + "Duplicated by Row: " + ((row.Table.Rows.IndexOf(row) + 1)).ToString() + "\n";
                                    DataRow tag = (DataRow)dgv[row[X].ToString(), w].Tag;
                                    row.RowError = "Duplicating Row: " + ((tag.Table.Rows.IndexOf(tag) + 1)).ToString();
                                }
                            }
                        }
                    }
                    //copy row collection toRow row tag
                    DataRow[] rowArray = new DataRow[rowcol.Count];
                    for (int i = 0; i < rowcol.Count; i++)
                    {
                        rowArray[i] = (DataRow)rowcol[i];
                    }
                    dgv.Rows[w].Tag = rowArray;
                }
                //copy column collection toRow column tag
                ArrayList colcol = new ArrayList();
                foreach (string xi in list)
                {
                    colcol.Clear();
                    foreach (DataGridViewRow row3 in dgv.Rows)
                    {
                        if (dgv[xi, row3.Index].Tag != null)
                        {
                            colcol.Add(dgv[xi, row3.Index].Tag);
                        }
                    }
                    DataRow[] colArray = new DataRow[colcol.Count];
                    for (int j = 0; j < colcol.Count; j++)
                    {
                        colArray[j] = (DataRow)colcol[j];
                    }
                    dgv.Columns[xi].Tag = colArray;
                }

                list.Clear();
                list2.Clear();
                colcol.Clear();
                rowcol.Clear();
            }
        }

        public static void New(int ZColumns, ref DataView view, String fieldX, String fieldY, ref DataGridView dgv)
        {
            Clean(ref dgv);

            if (view.Count != 0)
            {
                dgv.Tag = view;
                int num = 1;
                while (num <= ZColumns)
                {
                    dgv.Columns.Add(string.Empty, string.Empty);
                    num++;
                }
                dgv.Columns.Add(fieldY, fieldY);

                IEnumerable<DataRowView> enumerable = view.Cast<DataRowView>().ToList();

                IList<object> list = Dumb.HashFrom<object>(enumerable, fieldX);
                foreach (object xi in list) dgv.Columns.Add(xi.ToString(), xi.ToString());
                IList<object> list2 = Dumb.HashFrom<object>(enumerable, fieldY);

                int w = 0;
                foreach (object yi in list2)
                {
                    w = dgv.Rows.Add();

                    IEnumerable<DataRowView> Y = enumerable.Where(i => i.Row.Field<object>(fieldY).Equals(yi));

                    foreach (object xi in list)
                    {
                        DataGridViewCell cell1 = dgv[xi.ToString(), w];

                        IEnumerable<DataRowView> X = Y.Where(i => i.Row.Field<object>(fieldX).Equals(xi));

                        foreach (DataRowView rowV in X)
                        {
                            DataRow row = rowV.Row;

                            if (cell1.Tag == null)
                            {
                                cell1.ErrorText = null;
                                row.RowError = null;
                                cell1.Tag = row;
                                dgv[fieldY, w].Tag = row;
                                for (num = 0; num <= ZColumns; num++) dgv[num, w].Tag = row;
                                dgv[fieldY, w].Value = yi;
                            }
                            else
                            {
                                DataRow tag = (DataRow)cell1.Tag;
                                cell1.ErrorText += "Duplicated by Row: " + row.Table.Rows.IndexOf(row).ToString() + "\n";
                                tag.RowError += cell1.ErrorText + "\n";
                                row.RowError += "Duplicating Row: " + tag.Table.Rows.IndexOf(tag).ToString() + "\n";
                            }
                        }
                    }
                }

                list.Clear();
                list2.Clear();
            }
        }

        #endregion old?

        public class Paint
        {
            public static void ByCodes(ref DataGridView DGV, string CodesColumnName, int FirstBestValue, int SecondBestValue, int DGVStartColumnIndex)
            {
                IEnumerable<DataGridViewCell> cells = Selected(DGV, true);
                cells = cells.Where(CellsFromCol(DGVStartColumnIndex)).ToList();

                foreach (DataGridViewCell cell in cells)
                {
                    DataRow tag = (DataRow)cell.Tag;
                    if (tag.IsNull(CodesColumnName)) continue;
                    int code = Convert.ToInt32(tag[CodesColumnName]);
                    if (code == FirstBestValue) cell.Style.BackColor = Color.Honeydew;
                    else
                    {
                        if (code == SecondBestValue) cell.Style.BackColor = Color.Azure;
                        else cell.Style.BackColor = Color.MistyRose;
                    }
                }
                cells = null;
                DGV.ClearSelection();
            }

            public static void ByCodes(ref DataGridView DGV, string CodesColumnName, string ParentRelation, int FirstBestValue, int SecondBestValue, int DGVStartColumnIndex)
            {
                IEnumerable<DataGridViewCell> cells = Selected(DGV, true);
                cells = cells.Where(CellsFromCol(DGVStartColumnIndex)).ToList();
                foreach (DataGridViewCell cell in cells)
                {
                    DataRow tag = (DataRow)cell.Tag;
                    DataRow r = tag.GetParentRow(ParentRelation);
                    if (r == null) continue;
                    if (r.IsNull(CodesColumnName)) continue;
                    int code = Convert.ToInt32(r[CodesColumnName]);
                    if (code == FirstBestValue) cell.Style.BackColor = Color.Honeydew;
                    else
                    {
                        if (code == SecondBestValue) cell.Style.BackColor = Color.Azure;
                        else cell.Style.BackColor = Color.MistyRose;
                    }
                }
                cells = null;
                DGV.ClearSelection();
            }

            public static void ByGroup(ref DataGridViewColumn ColumnToGroup, int MinGreen)
            {
                DataGridView dataGridView = ColumnToGroup.DataGridView;
                Random random = new Random();
                int red = 0;
                int blue = 0;
                HashSet<string> set = new HashSet<string>();
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (set.Add(dataGridView[ColumnToGroup.Index, row.Index].Value.ToString()))
                    {
                        red = random.Next(MinGreen, 0xff);
                        blue = random.Next(MinGreen, 0xff);
                    }
                    dataGridView[ColumnToGroup.Index, row.Index].Style.BackColor = Color.FromArgb(red, 0, blue);
                    dataGridView[ColumnToGroup.Index, row.Index].Style.ForeColor = Color.White;
                }
                set.Clear();
            }

            public static void BySwitch(ref DataGridView DGV, string SwitchColumnName, int LessThan, Color ColorLessThan, Color ColorMoreThan, int DGVStartColumnIndex)
            {
                IEnumerable<DataGridViewCell> cells = Selected(DGV, true);
                cells = cells.Where(CellsFromCol(DGVStartColumnIndex)).ToList();
                foreach (DataGridViewCell cell in cells)
                {
                    DataRow tag = (DataRow)cell.Tag;
                    if (tag.IsNull(SwitchColumnName)) continue;
                    Int16 val = Convert.ToInt16(tag[SwitchColumnName]);
                    if (val < LessThan) // less than one for Gamma Reject
                    {
                        cell.Style.ForeColor = ColorLessThan;
                        cell.Style.SelectionForeColor = ColorLessThan;
                    }
                    else
                    {
                        cell.Style.ForeColor = ColorMoreThan;
                        cell.Style.SelectionForeColor = ColorMoreThan;
                    }
                }
                cells = null;
                DGV.ClearSelection();
            }

            public static void BySwitch(ref DataGridView DGV, string SwitchColumnName, int OffValue, Color ColorOff, int OnValue, Color ColorOn, int DGVStartColumnIndex)
            {
                IEnumerable<DataGridViewCell> cells = Selected(DGV, true);
                cells = cells.Where(CellsFromCol(DGVStartColumnIndex)).ToList();

                foreach (DataGridViewCell cell in cells)
                {
                    DataRow tag = (DataRow)cell.Tag;
                    if (tag.IsNull(SwitchColumnName)) continue;
                    Int16 val = Convert.ToInt16(tag[SwitchColumnName]);
                    if (val == OffValue)
                    {
                        cell.Style.ForeColor = ColorOff;
                        cell.Style.SelectionForeColor = ColorOff;
                    }
                    else if (val == OnValue)
                    {
                        cell.Style.ForeColor = ColorOn;
                        cell.Style.SelectionForeColor = ColorOn;
                    }
                }
                cells = null;
                DGV.ClearSelection();
            }

            public static void BackgroundBySwitch(ref DataGridView DGV, string SwitchColumnName, int OffValue, Color ColorOff, int OnValue, Color ColorOn, int DGVStartColumnIndex)
            {
                IEnumerable<DataGridViewCell> cells = Selected(DGV, true);
                cells = cells.Where(CellsFromCol(DGVStartColumnIndex)).ToList();

                foreach (DataGridViewCell cell in cells)
                {
                    DataRow tag = (DataRow)cell.Tag;
                    if (tag.IsNull(SwitchColumnName)) continue;
                    Int16 val = Convert.ToInt16(tag[SwitchColumnName]);

                    if (val == OffValue) cell.Style.BackColor = ColorOff;
                    else if (val == OnValue) cell.Style.BackColor = ColorOn;
                }
                DGV.ClearSelection();
            }

            public static void CellWithErrorColumn(ref DataGridView DGV, string ErrorColumnName, string ErrorToMatch, Color ColorToPut, int DGVColumnIndex)
            {
                ErrorToMatch = ErrorToMatch.ToUpper();
                IEnumerable<DataGridViewCell> cells = Selected(DGV, true);
                cells = cells.Where(CellsByCol(DGVColumnIndex));

                foreach (DataGridViewCell cell in cells)
                {
                    DataRow tag = (DataRow)((IEnumerable<DataRow>)cell.Tag).First();
                    if (tag.GetColumnError(ErrorColumnName).ToUpper().CompareTo(ErrorToMatch) == 0)
                    {
                        cell.Style.BackColor = ColorToPut;
                    }
                }

                DGV.ClearSelection();
            }
        }

        public class Tip
        {
            public static void AnyColumnCell(ref DataGridView DGV, string ColumnNameWithTipData, int DGVColumnToTip, string Comment)
            {
                IEnumerable<DataGridViewCell> cells = Selected(DGV, true);
                cells = cells.Where(CellsByCol(DGVColumnToTip));

                foreach (DataGridViewCell cell in cells)
                {
                    DataRow tag = (DataRow)cell.Tag;
                    if (tag.RowState != DataRowState.Deleted && tag.RowState != DataRowState.Detached)
                    {
                        cell.ToolTipText = Comment + tag[ColumnNameWithTipData].ToString();
                    }
                }
                DGV.ClearSelection();
            }

            public static void ColumnHeaderWithCommonRowDataFrom(ref DataGridView DGV, int DGVStartFromColumn, string Comment, string ColumnNameWithTipData)
            {
                HashSet<int> set = new HashSet<int>();
                IEnumerable<DataGridViewCell> cells = Selected(DGV, true);
                cells = cells.Where(CellsFromCol(DGVStartFromColumn));
                cells = cells.Where(o => set.Add(o.ColumnIndex)).ToList();
                foreach (DataGridViewCell cell in cells)
                {
                    DataRow tag = (DataRow)cell.Tag;
                    if (tag.IsNull(ColumnNameWithTipData)) continue;
                    string val = tag[ColumnNameWithTipData].ToString();
                    DataGridViewColumnHeaderCell headerCell = cell.OwningColumn.HeaderCell;
                    headerCell.ToolTipText = headerCell.ToolTipText + Comment + val + "\n";
                }
                set.Clear();
                set = null;
                cells = null;
                DGV.ClearSelection();
            }

            /// <summary>
            /// Latest
            /// </summary>
            /// <param name="DGV"></param>
            /// <param name="DGVStartFromColumn"></param>
            /// <param name="Comment">same lenght as parencolumnname</param>
            /// <param name="ParentRelation"></param>
            /// <param name="ParentColumnNameWithTipData"></param>
            public static void ColumnHeaderWithCommonRowDataFrom(ref DataGridView DGV, int DGVStartFromColumn, string[] Comment, string ParentRelation, string[] ParentColumnNameWithTipData)
            {
                HashSet<int> set = new HashSet<int>();
                IEnumerable<DataGridViewCell> cells = Selected(DGV, true);
                cells = cells.Where(CellsFromCol(DGVStartFromColumn));
                cells = cells.Where(o => set.Add(o.ColumnIndex)).ToList();

                foreach (DataGridViewCell cell in cells)
                {
                    DataRow tag = (DataRow)cell.Tag;
                    DataRow parent = tag.GetParentRow(ParentRelation);
                    if (parent == null) continue;

                    DataGridViewColumnHeaderCell headerCell = cell.OwningColumn.HeaderCell;
                    int subcount = ParentColumnNameWithTipData.Count();
                    for (int i = 0; i < subcount; i++)
                    {
                        string com = Comment[i];
                        string ptip = ParentColumnNameWithTipData[i];
                        string ptipval = parent[ptip].ToString();
                        headerCell.ToolTipText += com + ptipval + "\n";
                    }
                }
                set.Clear();
                set = null;
                cells = null;
                DGV.ClearSelection();
            }

            /// <summary>
            /// Latest
            /// </summary>
            /// <param name="DGV"></param>
            /// <param name="DGVStartFromColumn"></param>
            /// <param name="Comment"></param>
            /// <param name="ParentRelation"></param>
            /// <param name="ParentColumnNameWithTipData"></param>
            public static void ColumnHeaderWithCommonTimeDurationFrom(ref DataGridView DGV, int DGVStartFromColumn, string Comment, string ParentRelation, string ParentColumnNameWithTipData)
            {
                HashSet<int> set = new HashSet<int>();
                IEnumerable<DataGridViewCell> cells = Selected(DGV, true);
                cells = cells.Where(CellsFromCol(DGVStartFromColumn));
                cells = cells.Where(o => set.Add(o.ColumnIndex)).ToList();

                foreach (DataGridViewCell cell in cells)
                {
                    DataRow tag = (DataRow)cell.Tag;

                    DataRow parent = tag.GetParentRow(ParentRelation);
                    if (parent == null) continue;
                    if (parent.IsNull(ParentColumnNameWithTipData)) continue;
                    double time = Convert.ToDouble(parent[ParentColumnNameWithTipData]);
                    TimeSpan span = TimeSpan.FromSeconds(time);
                    string str = Dumb.ToReadableString(span);
                    DataGridViewColumnHeaderCell headerCell = cell.OwningColumn.HeaderCell;
                    headerCell.ToolTipText = headerCell.ToolTipText + Comment + str + "\n";
                }
                set.Clear();
                set = null;
                cells = null;
                DGV.ClearSelection();
            }
        }
    }
}