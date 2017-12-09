﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace TrafficSimDataAnalyzer
{
    public partial class Form1 : Form
    {

        XmlDocument doc = new XmlDocument();
       
        XmlElement root;

        public Form1()
        {
            InitializeComponent();
        }

        private void selectXmlB_Click(object sender, EventArgs e)
        {

            XmlElement oldEl = null;
            XmlNode oldN = null;
            chart.Series.Clear();
            chart.ResetAutoValues();
            Series s = new Series();
            s.ChartType = SeriesChartType.Line;
            foreach (XmlElement el in root.ChildNodes)
            {
                if (oldEl != null && oldEl.ChildNodes.Count > 0 && float.Parse(el.ChildNodes[1].InnerText) < float.Parse(oldEl.ChildNodes[1].InnerText))
                {
                    chart.Series.Add(s);
                    s = new Series();
                    s.ChartType = SeriesChartType.Line;
                }
                foreach (XmlNode n in el.ChildNodes)
                {
                    if(n.Name == featureName.Text)
                    {
                        s.Name = $"{featureName.Text}: {float.Parse(n.InnerText)} Trial: {chart.Series.Count}";
                        float yVal = 0;
                        float throughput = float.Parse(el.ChildNodes[el.ChildNodes.Count - 1].InnerText);
                        float crashes = float.Parse(el.ChildNodes[el.ChildNodes.Count - 2].InnerText);
                        if (RatioBox.SelectedIndex == 0)
                        {
                            yVal = throughput / (crashes == 0 ? 1 : crashes);
                        }
                        else if (RatioBox.SelectedIndex == 1)
                        {
                            yVal = crashes / (throughput == 0 ? 1: throughput);
                        }
                        else if(RatioBox.SelectedIndex == 2)
                        {
                            yVal = crashes / (throughput + crashes);
                        }
                        else if(RatioBox.SelectedIndex == 3)
                        {
                            yVal = throughput / (throughput + crashes);
                        }
                        s.Points.Add(new DataPoint(float.Parse(el.ChildNodes[1].InnerText), yVal));
                        //if (oldN != null && (n.InnerText != oldN.InnerText))
                        //{
                            
                        //    chart.Series.Add(s);
                        //    s = new Series();
                        //    s.ChartType = SeriesChartType.Line;
                        //}
                        oldN = n;
                    }
                }
                
                
                oldEl = el;
            }
            chart.Series.Add(s);
        }

        private void chart_Click(object sender, EventArgs e)
        {
            
        }

        private void trialSelect_Click(object sender, EventArgs e)
        {
            int trialsCounted = 0;
            int trialTarget = int.Parse(trialNumBox.Text);
            foreach(XmlElement child in root.ChildNodes)
            {
                if (child.HasChildNodes && child.ChildNodes.Count >= 2)
                {
                    if (child.ChildNodes[1].InnerText == "0.5")
                    {
                        trialsCounted++;
                    }
                    if (trialsCounted >= trialTarget)
                    {
                        StringBuilder output = new StringBuilder();
                        foreach(XmlNode el in child.ChildNodes)
                        {
                            output.Append(el.Name);
                            output.AppendLine(": " + el.InnerText);
                        }
                        trialXML.Text = output.ToString();
                        break;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            doc.Load("Data.xml");
            root = doc.DocumentElement;
        }
    }
}
