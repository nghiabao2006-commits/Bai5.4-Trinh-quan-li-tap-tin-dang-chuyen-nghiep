using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bai5._4
{
    public partial class Form1 : Form
    {
        class Employee
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public string Position { get; set; }
            public DateTime StartDate { get; set; }
            public string DepartmentPath { get; set; } // e.g., "Company/Dept/Group"
        }

        private List<Employee> _employees = new List<Employee>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BuildSampleData();
            PopulateTree();
            // prepare ListView columns for Details
            lsvEmployees.Columns.Clear();
            lsvEmployees.Columns.Add("Mã NV", 80);
            lsvEmployees.Columns.Add("Họ Tên", 180);
            lsvEmployees.Columns.Add("Chức vụ", 120);
            lsvEmployees.Columns.Add("Ngày vào làm", 120);

            // prepare images
            imageListSmall.Images.Clear();
            imageListLarge.Images.Clear();
            // use system icons converted to bitmaps
            try
            {
                imageListSmall.Images.Add("company", SystemIcons.Application.ToBitmap());
                imageListSmall.Images.Add("dept", SystemIcons.WinLogo.ToBitmap());
                imageListSmall.Images.Add("group", SystemIcons.Information.ToBitmap());
                imageListLarge.Images.Add("person", SystemIcons.Shield.ToBitmap());
            }
            catch { }

            // set default view
            cmbView.SelectedIndex = 0; // Details
            lsvEmployees.View = View.Details;
        }

        private void BuildSampleData()
        {
            _employees.Clear();
            // Company: TechCorp
            _employees.Add(new Employee { Id = "E001", FullName = "Nguyễn Văn A", Position = "Trưởng phòng", StartDate = new DateTime(2018, 3, 1), DepartmentPath = "TechCorp/IT/Platform" });
            _employees.Add(new Employee { Id = "E002", FullName = "Trần Thị B", Position = "Lập trình viên", StartDate = new DateTime(2019, 6, 15), DepartmentPath = "TechCorp/IT/Platform" });
            _employees.Add(new Employee { Id = "E003", FullName = "Lê Văn C", Position = "Kiểm thử", StartDate = new DateTime(2020, 1, 20), DepartmentPath = "TechCorp/IT/QA" });
            _employees.Add(new Employee { Id = "E004", FullName = "Phạm Thị D", Position = "Nhân viên kinh doanh", StartDate = new DateTime(2017, 11, 5), DepartmentPath = "TechCorp/Sales/Inside" });
            _employees.Add(new Employee { Id = "E005", FullName = "Hoàng Văn E", Position = "Giám đốc", StartDate = new DateTime(2015, 2, 10), DepartmentPath = "TechCorp/Management" });
            _employees.Add(new Employee { Id = "E006", FullName = "Đỗ Thị F", Position = "Lập trình viên", StartDate = new DateTime(2021, 9, 1), DepartmentPath = "TechCorp/IT/Platform" });
        }

        private void PopulateTree()
        {
            tvDepartments.Nodes.Clear();
            // Build tree from employee DepartmentPath values
            TreeNode root = new TreeNode("TechCorp") { Tag = "TechCorp", ImageKey = "company", SelectedImageKey = "company" };
            foreach (var emp in _employees)
            {
                var parts = emp.DepartmentPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;
                // start from root
                TreeNode current = root;
                // skip first part if it's the company name (TechCorp)
                int start = parts[0].Equals("TechCorp", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
                for (int i = start; i < parts.Length; i++)
                {
                    string p = parts[i];
                    TreeNode next = null;
                    foreach (TreeNode n in current.Nodes)
                    {
                        if (n.Text == p) { next = n; break; }
                    }
                    if (next == null)
                    {
                        next = new TreeNode(p) { Tag = string.Join("/", parts.Take(i + 1)) };
                        // set images for dept/group
                        next.ImageKey = i == 0 ? "dept" : "group";
                        next.SelectedImageKey = next.ImageKey;
                        current.Nodes.Add(next);
                    }
                    current = next;
                }
            }
            tvDepartments.Nodes.Add(root);
            root.Expand();
        }

        private void tvDepartments_AfterSelect(object sender, TreeViewEventArgs e)
        {
            LoadEmployeesForNode(e.Node);
        }

        private void LoadEmployeesForNode(TreeNode node)
        {
            lsvEmployees.Items.Clear();
            if (node == null) return;
            // determine the path to filter by
            string path = node.Tag as string;
            if (string.IsNullOrEmpty(path))
            {
                // root node may have Text but no Tag; use Text as company
                path = node.Text;
            }

            // match employees where DepartmentPath equals or starts with path
            var matched = _employees.Where(emp => emp.DepartmentPath.Equals(path, StringComparison.OrdinalIgnoreCase)
                || emp.DepartmentPath.StartsWith(path + "/", StringComparison.OrdinalIgnoreCase)
                || path.Equals("TechCorp", StringComparison.OrdinalIgnoreCase) && emp.DepartmentPath.StartsWith("TechCorp/"));

            foreach (var emp in matched)
            {
                ListViewItem item = new ListViewItem(emp.Id);
                item.SubItems.Add(emp.FullName);
                item.SubItems.Add(emp.Position);
                item.SubItems.Add(emp.StartDate.ToString("dd/MM/yyyy"));
                item.ImageKey = "person";
                lsvEmployees.Items.Add(item);
            }
            // adjust column widths
            if (lsvEmployees.View == View.Details)
            {
                for (int i = 0; i < lsvEmployees.Columns.Count; i++)
                {
                    lsvEmployees.Columns[i].Width = -2; // autosize
                }
            }
        }

        private void cmbView_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbView.SelectedItem as string)
            {
                case "Details": lsvEmployees.View = View.Details; break;
                case "LargeIcon": lsvEmployees.View = View.LargeIcon; break;
                case "SmallIcon": lsvEmployees.View = View.SmallIcon; break;
                case "List": lsvEmployees.View = View.List; break;
                case "Tile": lsvEmployees.View = View.Tile; break;
                default: lsvEmployees.View = View.Details; break;
            }
        }
    }
}
