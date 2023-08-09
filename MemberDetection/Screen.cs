using Autodesk.Navisworks.Api;
using Intratech.Cores;
using Intratech.Cores.Control;
using Intratech.PRC.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace MemberDetection
{
    public partial class Screen : Form
    {
        public Screen()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            InitializeComponent();
        }
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //MessageBox.Show(args.Name);
            return null;
        }

        private void btnExportGeometry_Click(object sender, EventArgs e)
        {
            var doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            ModelItemCollection modelItems = doc.CurrentSelection.SelectedItems;

            string filePath = "C:\\Users\\AnhTu\\Member Structure Detection\\MemberDetection\\MemberDetection\\Geometry.obj";
            ExportGeometry exportGeometry = new ExportGeometry(modelItems);
            exportGeometry.exportGeometry(filePath);
        }

        private void btnDetectType_Click(object sender, EventArgs e)
        {
            var doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            ModelItemCollection modelItems = doc.CurrentSelection.SelectedItems;
            DataGeometry dataGeometry = new DataGeometry();
            dataGeometry = dataGeometry.getGeometry(modelItems);

            try
            {
                Modulize.InitializeSDK(@"AAAAAAAAAAAAAAAAAAAAADCJ1vscZcjeUpXlyb6rBH+Z/H90/YkLYLtaGVvOPYMxUNes7v+VkO8yog3x9zIwcFq1hGXoQ2m4HGD6L/VlcbLPN+dlelq2bogTYxTQNf/6Z4X6jx2J5lbNbIJpec4TbpWuD9oV094lq9lPdxaZUEJxWxdwzSKymCQCxb3GE9WZrQ9u33NbpcXbTqf0ul6Sew==");
                Modulize.InitializeSDK(@"AAAAAAAAAAAAAAAAAAAAADCJ1vscZcjeUpXlyb6rBH+Z/H90/YkLYLtaGVvOPYMxUNes7v+VkO8yog3x9zIwcFq1hGXoQ2m4HGD6L/VlcbLPN+dlelq2bogTYxTQNf/6Z4X6jx2J5lbNbIJpec4TbpWuD9oV094lq9lPdxaZUEJxWxdwzSKymCQCxb3GE9WZQoFJPtEEGpm618lZHF+WAQ==");

                Writer prcWriter = new Writer(1);

                ModelItem selectedItem = null;
                foreach (var item in modelItems)
                {
                    selectedItem = item;
                }

                WriteToPDF.recursive(prcWriter, selectedItem, dataGeometry, null);

                WriteToPDF.SaveToPDF(prcWriter);
                //MessageBox.Show("Save PDF successfully");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            MemberPartDetector memberPartDetector = new MemberPartDetector();
            MemberType type = memberPartDetector.getType(dataGeometry);
            MessageBox.Show($"The type of selected item is {type.ToString()}");
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            ModelItemCollection modelItems = doc.CurrentSelection.SelectedItems;
            DataGeometry dataGeometry = new DataGeometry();
            dataGeometry = dataGeometry.getGeometry(modelItems);
            sw.Stop();

            sw.Start();
            MemberPartDetector memberPartDetector = new MemberPartDetector();
            memberPartDetector.detect(in dataGeometry, out MemberType memberType, out Vector3? firstCenter, out Vector3? secondCenter, out double? maxDistance);
            sw.Stop();
            this.richTextBox.Text = $"The process time is: {sw.ElapsedMilliseconds.ToString()} ms!";

            try
            {
                Modulize.InitializeSDK(@"AAAAAAAAAAAAAAAAAAAAADCJ1vscZcjeUpXlyb6rBH+Z/H90/YkLYLtaGVvOPYMxUNes7v+VkO8yog3x9zIwcFq1hGXoQ2m4HGD6L/VlcbLPN+dlelq2bogTYxTQNf/6Z4X6jx2J5lbNbIJpec4TbpWuD9oV094lq9lPdxaZUEJxWxdwzSKymCQCxb3GE9WZrQ9u33NbpcXbTqf0ul6Sew==");
                Modulize.InitializeSDK(@"AAAAAAAAAAAAAAAAAAAAADCJ1vscZcjeUpXlyb6rBH+Z/H90/YkLYLtaGVvOPYMxUNes7v+VkO8yog3x9zIwcFq1hGXoQ2m4HGD6L/VlcbLPN+dlelq2bogTYxTQNf/6Z4X6jx2J5lbNbIJpec4TbpWuD9oV094lq9lPdxaZUEJxWxdwzSKymCQCxb3GE9WZQoFJPtEEGpm618lZHF+WAQ==");

                Writer prcWriter = new Writer(1);

                ModelItem selectedItem = null;
                foreach (var item in modelItems)
                {
                    selectedItem = item;
                }

                List<Vector3?> centerPoints = new List<Vector3?>() { firstCenter, secondCenter };
                WriteToPDF.recursive(prcWriter, selectedItem, dataGeometry, centerPoints);

                WriteToPDF.SaveToPDF(prcWriter);
                //MessageBox.Show("Save PDF successfully");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
    }
}
