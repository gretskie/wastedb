using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WasteApp.classes
{
    class graph
    {
        private ScottPlot.Plot plt;
        private WpfPlot chart;
        private double[] values;
        private string[] labels;


        public graph(WpfPlot chart)
        {
            this.chart = chart;
            this.plt = chart.Plot;
        }

        public graph(WpfPlot chart, double[] values, string[] labels)
        {
            this.chart = chart;
            this.plt = this.chart.Plot;
            this.values = values;
            this.labels = labels;
        }

        public graph(WpfPlot chart, List<currentActivityTotalByIsotope> list)
        {
            this.chart = chart;
            this.plt = this.chart.Plot;
            values = new double[list.Count];
            labels = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                values[i] = list[i].percentOfLimit;
                labels[i] = list[i].isotopeSymbol.ToString() + ": " + list[i].totalActivity.ToString("N0") +"MBq\nLimit : "+ list[i].limitQuant.ToString();
            }
        }

        public void updateValues(double[] values, string[] labels)
        {
            this.values = values;
            this.labels = labels;
        }

        public void updateValues(List<currentActivityTotalByIsotope> list)
        {
            values = new double[list.Count];
            labels = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                values[i] = list[i].percentOfLimit;
                labels[i] = list[i].isotopeSymbol.ToString() + ": " + list[i].totalActivity.ToString("N0") + "MBq";
            }
        }

        public void displaySummaryGraph()
        {
            plt.Clear();
            List<ScottPlot.Plottable.Bar> bars = new List<ScottPlot.Plottable.Bar>();

            for(int i = 0; i < this.values.Length; i++)
            {
                ScottPlot.Plottable.Bar bar = new ScottPlot.Plottable.Bar();

                bar.Value = this.values[i]*100;
                bar.Position = i;
                if(this.values[i]*100 < 80)
                {
                    bar.FillColor = System.Drawing.Color.Green;
                }
                else
                {
                    bar.FillColor = System.Drawing.Color.Red;
                }
                bar.Label = labels[i];
                bar.LineWidth = 2;
                bars.Add(bar);
            }
            this.plt.AddBarSeries(bars);
            this.plt.YLabel("Percent of Limit");
            this.plt.XAxis.Ticks(false);
            this.plt.SetAxisLimitsY(0, 110);
            this.chart.Refresh();
    
        }
    }
}
