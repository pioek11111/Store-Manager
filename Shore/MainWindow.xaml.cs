using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Shore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Product> removed;
        List<Product> listNotBinded;
        Products products;
        ProductsToSearch productsToSearch;
        Cart cart;
        const string c = "qwertyuiop";
        ICollectionView view, view2;
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitBinding();
        }
        private void InitBinding()
        {
            removed = new List<Product>();
            listNotBinded = new List<Product>();
            products = new Products();
            productsToSearch = new ProductsToSearch();
            cart = new Cart();
            view = (CollectionView)new CollectionViewSource { Source = products }.View;
            lstPersons.ItemsSource = view;
            view2 = (CollectionView)new CollectionViewSource { Source = products }.View;
            WPersons.ItemsSource = view2;
            cartListBox.ItemsSource = cart;
            textBlock.DataContext = cart.totalPrice;
        }
        private void btnSimpleMessageBox_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This is simple shop manager aplication", null, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void clear(object sender, RoutedEventArgs e)
        {
            products.Clear();            
        }
        private void add(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                products.Add(new Product("Computer", "Lorem ipsum dolor sit amet enim. Etiam ullamcorper. Suspendisse a pellentesque dui, non felis.", 2499, Product.category.Electronics));
                products.Add(new Product("Apple", "Lorem ipsum dolor sit amet enim. Etiam ullamcorper. Suspendisse a pellentesque dui, non felis.", 1.6, Product.category.Food));
            }
            
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            textBox.IsEnabled = true;
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }
        private void button_Click(object sender, RoutedEventArgs e)
        {            
            if (checkBox.IsChecked == true)
            {
                string name = textBox.Text;
                
                view.Filter = item =>
                {
                    Product vitem = item as Product;
                    if (vitem == null) return false;

                    return vitem.Name.Contains(name);
                };
            }
            else if (checkBox1.IsChecked == true)
            {
                try
                {
                    int from = Convert.ToInt32(textBox1.Text);
                    int to = Convert.ToInt32(textBox2.Text);
                    view.Filter = item =>
                    {
                        Product vitem = item as Product;
                        if (vitem == null) return false;

                        return vitem.Price <= to && vitem.Price >= from;
                    };
                }
                catch
                {
                    MessageBox.Show("Invalid format of price", null, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else if (checkBox2.IsChecked == true)
            {
                view.Filter = item =>
                {
                    Product vitem = item as Product;
                    if (vitem == null) return false;
                    var c = comboBox;
                    return vitem.Category.ToString() == c.SelectionBoxItem.ToString();
                };
            }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            view.Filter = null;
            view.Refresh();                            
            view = CollectionViewSource.GetDefaultView(products);
            lstPersons.ItemsSource = view;
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            textBox1.IsEnabled = true;
            textBox2.IsEnabled = true;
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            textBox1.IsEnabled = false;
            textBox2.IsEnabled = false;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            textBox.IsEnabled = false;
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            comboBox.IsEnabled = true;
        }

        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            comboBox.IsEnabled = false;
        }

        private void checkout(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Total Price: " + cart.totalPrice.ToString("c"), null, MessageBoxButton.OK);
        }

        private void plus(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ElementOfCart p = button.DataContext as ElementOfCart;
            p.quantity++;
            cart.totalPrice += p.product.Price;
            textBlock.DataContext = cart.totalPrice;
            cartListBox.ItemsSource = null;
            cartListBox.ItemsSource = cart;
        }
        private void minus(object sender, RoutedEventArgs e)
        {            
            Button button = sender as Button;
            ElementOfCart p = button.DataContext as ElementOfCart;
            if (p.quantity > 0)
            {
                p.quantity--;
                cart.totalPrice -= p.product.Price;
                textBlock.DataContext = cart.totalPrice;
                cartListBox.ItemsSource = null;
                cartListBox.ItemsSource = cart;
            }
        }

        private void saveAllProducts(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Data Files (*.xml*)|*.xml*";
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.ShowDialog();
            XmlSerializer s = new XmlSerializer(typeof(Products));
            try
            {
                TextWriter WriteFileStream = new StreamWriter(saveFileDialog.FileName);
                s.Serialize(WriteFileStream, products);
            }
            catch { }
        }
        private void loadProductsFromXMLFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Data Files (*.xml*)|*.xml*";
            openFileDialog.DefaultExt = "xml";
            openFileDialog.ShowDialog();
            try
            {
                FileStream ReadFileStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                XmlSerializer s = new XmlSerializer(typeof(Products));
                Products p = (Products)s.Deserialize(ReadFileStream);
                foreach (var item in p)
                {
                    products.Add(item);
                }
            }
            catch { }
        }

        private void removeFromCart(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ElementOfCart p = button.DataContext as ElementOfCart;
            
            cart.totalPrice -= p.product.Price * p.quantity;
            textBlock.DataContext = cart.totalPrice;
            cartListBox.ItemsSource = null;
            cartListBox.ItemsSource = cart;
            cart.Remove(p);
        }

        private void lstPersons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void addToCart(object sender, RoutedEventArgs e)
        {           
            var a = sender as Product;
            Button button = sender as Button;
            Product p = button.DataContext as Product;
            Window1 w = new Window1(p.Category);
            w.ShowInTaskbar = false;
            w.Owner = Application.Current.MainWindow;
            w.ShowDialog();
            if (w.DialogResult == true)
            {
                int q;
                try
                {
                    q = Convert.ToInt32(w.textBlock.Text);
                    cart.totalPrice += q * p.Price;
                    textBlock.DataContext = cart.totalPrice;
                    foreach (var item in cart)
                    {
                        if (item.product == p)
                        {
                            var el = new ElementOfCart(p, item.quantity + q);                            
                            cart.Remove(item);
                            cart.Add(el);
                            return;
                        }
                    }
                    cart.Add(new ElementOfCart(p,q));
                }
                catch
                {
                    MessageBox.Show("Invalid format of price", null, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                w.Close();
            }
        }
    }

    public class Product
    {
        public Product()
        {
            
        }
        
        public Product(string n, string d, double p, category c)
        {
            Name = n;
            Description = d;
            Price = p;
            Category = c;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public category Category { get; set; }
        public override string ToString()
        {
            return Name.ToString();
        }
        public enum category
        {
            Food,
            Cloths,
            Electronics
        }
    }
    public class Products : ObservableCollection<Product>
    {
        public Products()
        {

        }
        public Products(List<Product> p)
        {
            foreach (var item in p)
            {
                Add(item);
            }
        }
    }
    public class ProductsToSearch : ObservableCollection<Product>
    {
        public ProductsToSearch()
        {
            //Add(new Product("Apple" , "rfevswv" , 2.5, 1));
        }
        public ProductsToSearch(Products p)
        {
            foreach (var item in p)
            {
                Add(item);
            }
        }            
    }
    public class ElementOfCart
    {
        public ElementOfCart(Product p, int q = 1)
        {
            product = p;
            quantity = q;
        }
        public Product product { get; set; }
        public int quantity { get; set; }
    }
    public class Cart : ObservableCollection<ElementOfCart>
    {         
        private double myVar;

        public double totalPrice
        {
            get { return myVar; }
            set { myVar = value; }
        }

        public Cart()
        {
            totalPrice = 0;
        }
    }
}