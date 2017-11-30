using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace TowerDefenceEF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<SQL.Typ_Budynku> listTypBudynku; 

        public MainWindow()
        {
            InitializeComponent();

            readDatabase();
        }

        private void readDatabase()
        {
            listTypBudynku = new List<SQL.Typ_Budynku>();

            using (SQL.TowerDefenceDataContext db = new SQL.TowerDefenceDataContext())
            {
                var data = from t in db.Typ_Budynkus
                           select new
                           {
                               Id = t.IdTypuBudynku,
                               Nazwa = t.Nazwa,
                               Max_Lvl = t.MaksymalnyPoziomRozwoju,
                               Zasieg = t.ZasiegAtaku,
                               Szybkosc = t.SzybkoscAtaku
                           };

                foreach (var detail in data)
                {
                    SQL.Typ_Budynku tmp = new SQL.Typ_Budynku();
                    tmp.IdTypuBudynku = detail.Id;
                    tmp.Nazwa = detail.Nazwa;
                    tmp.MaksymalnyPoziomRozwoju = detail.Max_Lvl;
                    tmp.ZasiegAtaku = detail.Zasieg;
                    tmp.SzybkoscAtaku = detail.Szybkosc;
                    listTypBudynku.Add(tmp);
                }

                dataGrid.ItemsSource = data;
            }
        }

        private void dodajNowyTypBudynku(object sender, RoutedEventArgs e)
        {
            if(tB_nazwa_insert.Text.Length != 0 && tB_mpr_insert.Text.Length != 0 && tB_za_insert.Text.Length != 0 && tB_sa_insert.Text.Length != 0)
            {
                string nazwa = tB_nazwa_insert.Text;
                int mpr;
                bool x1 = int.TryParse(tB_mpr_insert.Text,out mpr);
                float za;
                bool x2 = float.TryParse(tB_za_insert.Text, out za);
                float sa;
                bool x3 = float.TryParse(tB_sa_insert.Text, out sa);
                if (x1 && x2 && x3)
                {
                    using (SQL.TowerDefenceDataContext db = new SQL.TowerDefenceDataContext())
                    {
                        SQL.Typ_Budynku ord = new SQL.Typ_Budynku
                        {
                            Nazwa = nazwa,
                            MaksymalnyPoziomRozwoju = mpr,
                            ZasiegAtaku = za,
                            SzybkoscAtaku = sa
                        };
                        
                        db.Typ_Budynkus.InsertOnSubmit(ord);

                        try
                        {
                            db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Błąd: " + ex.Message);
                            db.SubmitChanges();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Błąd: Niepoprawne typy danych");
                }
            }
            else
            {
                MessageBox.Show("Błąd: Pusta wartość");
            }

            readDatabase();

            dataGrid.SelectedItems.Clear();
            
            object item = dataGrid.Items[listTypBudynku.Count-1];
            dataGrid.SelectedItem = item;

            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(listTypBudynku.Count - 1) as DataGridRow;
            if (row == null)
            {
                dataGrid.ScrollIntoView(item);
                row = dataGrid.ItemContainerGenerator.ContainerFromIndex(listTypBudynku.Count - 1) as DataGridRow;
            }
        }

        private void usunTypBudynku(object sender, RoutedEventArgs e)
        {
            if (tB_id_delete.Text.Length != 0)
            {
                int id;
                bool x0 = int.TryParse(tB_id_delete.Text, out id);
                if (x0)
                {
                    using (SQL.TowerDefenceDataContext db = new SQL.TowerDefenceDataContext())
                    {
                        var deleteOrderDetails =
                            from details in db.Typ_Budynkus
                            where details.IdTypuBudynku == id
                            select details;

                        if (deleteOrderDetails.Count<SQL.Typ_Budynku>() == 0)
                        {
                            MessageBox.Show("Błąd: Wiersz o podanym Id nie istnieje");
                            return;
                        }

                        foreach (var detail in deleteOrderDetails)
                        {
                            db.Typ_Budynkus.DeleteOnSubmit(detail);
                        }

                        try
                        {
                            db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Błąd: " + ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("ERROR: TYPE");
                }
            }
            else
            {
                MessageBox.Show("ERROR: NULL");
            }

            readDatabase();
        }

        private void zaktualizujTypBudynku(object sender, RoutedEventArgs e)
        {
            if (tB_id_update.Text.Length != 0 && (tB_nazwa_update.Text.Length != 0 || tB_mpr_update.Text.Length != 0 || tB_za_update.Text.Length != 0 || tB_sa_update.Text.Length != 0))
            {
                int id;
                bool x0 = int.TryParse(tB_id_update.Text, out id);
                string nazwa = tB_nazwa_update.Text;
                int mpr;
                bool x1 = int.TryParse(tB_mpr_update.Text, out mpr);
                float za;
                bool x2 = float.TryParse(tB_za_update.Text, out za);
                float sa;
                bool x3 = float.TryParse(tB_sa_update.Text, out sa);
                if (x0 && (x1 || x2 || x3 || tB_nazwa_update.Text.Length != 0))
                {
                    using (SQL.TowerDefenceDataContext db = new SQL.TowerDefenceDataContext())
                    {
                        var query =
                            from ord in db.Typ_Budynkus
                            where ord.IdTypuBudynku == id
                            select ord;

                        if (query.Count<SQL.Typ_Budynku>() == 0)
                        {
                            MessageBox.Show("Błąd: Wiersz o podanym Id nie istnieje");
                            return;
                        }
                        
                        foreach (SQL.Typ_Budynku ord in query)
                        {
                            if (tB_nazwa_update.Text.Length != 0) ord.Nazwa = nazwa;
                            if (x1) ord.MaksymalnyPoziomRozwoju = mpr;
                            if (x2) ord.ZasiegAtaku = za;
                            if (x3) ord.SzybkoscAtaku = sa;
                        }
                        
                        try
                        {
                            db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Błąd: "+ex.Message);
                        }
                    }

                    readDatabase();

                    dataGrid.SelectedItems.Clear();

                    int index = 0;
                    for(int i=0; i<listTypBudynku.Count; i++)
                    {
                        if (listTypBudynku[i].IdTypuBudynku == id)
                        {
                            index = i;
                            break;
                        }
                    }

                    object item = dataGrid.Items[index];
                    dataGrid.SelectedItem = item;

                    DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
                    if (row == null)
                    {
                        dataGrid.ScrollIntoView(item);
                        row = dataGrid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
                    }
                }
                else
                {
                    MessageBox.Show("ERROR: TYPE");
                }
            }
            else
            {
                MessageBox.Show("ERROR: NULL");
            }
        }

        private void zaznaczDoEdycji(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                string s = dataGrid.SelectedItem.ToString();
                string[] tab = s.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                string Sid = Regex.Replace(tab[0], "[^0-9]", "");
                tB_id_delete.Text = Sid;
                tB_id_update.Text = Sid; 
            }
        }
    }
}
