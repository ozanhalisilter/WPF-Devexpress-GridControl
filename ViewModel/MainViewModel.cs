using DevExpress.Data.XtraReports.Native;
using DevExpress.Xpf.CodeView;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Mvvm.Native;
using WPF_Devexpress_GridControl.Model;
using DevExpress.Mvvm;
using DevExpress.Data;
using DevExpress.Mvvm.POCO;
using System.Collections.Specialized;

namespace WPF_Devexpress_GridControl.ViewModel
{


    internal class MainViewModel : ViewModelBase, INotifyPropertyChanged

    {

        //DB Connection
        public string query = "SELECT * FROM userlist;";
        string connectionString = "SERVER=localhost;DATABASE=users;UID=root;PASSWORD=5858";


        private string logBlock = "";
        public string LogBlock
        {
            get
            {
                return logBlock;
            }

            set
            {
                if (logBlock == value)
                {
                    return;
                }

                logBlock = value;
                OnPropertyChanged("LogBlock");
            }
        }


        MySqlConnection conn;
        static Database dbObject = new Database();

        //Sync locks
        private object _syncLock = new Object();
        private object _syncLockNotCollection = new Object();

        //Obsvbl Collections

        ObservableCollection<Person> userList;
        ObservableCollection<Person> userListNotCollection;
        ObservableCollection<Person> newList;
        public ObservableCollection<Person> UserList
        {
            get { return userList; }
            set
            {
                userList = value;
                OnPropertyChanged("UserList");
            }
        }
        public ObservableCollection<Person> UserListNotCollection
        {
            get { return userListNotCollection; }
            set
            {
                userListNotCollection = value;
                OnPropertyChanged("UserListNotCollection");
            }
        }
        public ObservableCollection<Person> NewList
        {
            get { return newList; }
            set
            {
                newList = value;
            }
        }
        public ICollectionView Collection { get; set; }

        //Relay Commands
        public ICommand AddMore { get; set; }
        public ICommand Clear { get; set; }
        public ICommand FetchFreeze { get; set; }
        public ICommand FetchOneByOneNoFreeze { get; set; }
        public ICommand FetchAllNoFreeze { get; set; }


        static Random random = new Random();


        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            conn = new MySqlConnection(connectionString);


            //Relay methods
            Clear = new RelayCommand(ClearRelay);
            FetchFreeze = new RelayCommand(FetchFreezeRelay);
            FetchOneByOneNoFreeze = new RelayCommand(FetchOneByOneNoFreezeRelay);
            FetchAllNoFreeze = new RelayCommand(FetchAllNoFreezeRelay);

            //Observable Collections
            UserList = new ObservableCollection<Person>();
            NewList = new ObservableCollection<Person>();
            UserListNotCollection = new ObservableCollection<Person>();

            //Wrapping ObservableCollection with Collection
            Collection = CollectionViewSource.GetDefaultView(UserList);
            BindingOperations.EnableCollectionSynchronization(UserList, _syncLock);


        }

        public void FetchFreezeRelay(object obj)
        {
            var thread = new Thread(() =>
            {

                Thread.CurrentThread.IsBackground = true;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                LogBlock = $"Entering loop {watch.ElapsedMilliseconds} \n";
                MySqlCommand cmd = new MySqlCommand(query, conn);


                lock (_syncLock)
                {
                    UserList.Clear();
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserList.Add(new Person((int)reader[0], reader[1].ToString(), reader[2].ToString()));
                        }
                    }
                    conn.Close();
                }
                watch.Stop();
                LogBlock += $"Finito {watch.ElapsedMilliseconds}  \n";

            });
            thread.Start();
            thread.Join();
            Collection.Refresh();
        }
        public void FetchAllNoFreezeRelay(object obj) //:D
        {
            var newThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                var watch = System.Diagnostics.Stopwatch.StartNew();

                MySqlCommand cmd = new MySqlCommand(query, conn);


                LogBlock = $"Entering loop {watch.ElapsedMilliseconds} \n";


                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    NewList.Clear();
                    while (reader.Read())
                    {
                        NewList.Add(new Person((int)reader[0], reader[1].ToString(), reader[2].ToString()));
                    }
                }
                conn.Close();

                LogBlock += $"Printing to screen {watch.ElapsedMilliseconds} \n";
                UserListNotCollection = new ObservableCollection<Person>(NewList);


                LogBlock += $"Finito {watch.ElapsedMilliseconds}  \n";

                watch.Stop();
            });
            newThread.Start();
        }
        public void FetchOneByOneNoFreezeRelay(object obj) //:D
        {

            Thread th = new Thread(() =>
            {

                var watch = System.Diagnostics.Stopwatch.StartNew();
                Thread.CurrentThread.IsBackground = true;

                MySqlCommand cmd = new MySqlCommand(query, conn);

                LogBlock = $"Entering loop {watch.ElapsedMilliseconds} \n";
                DataTable table = new DataTable();

                conn.Open();


                LogBlock += $"Connected DB {watch.ElapsedMilliseconds} \n";
                using (var reader = cmd.ExecuteReader())

                {
                    lock (_syncLock)
                    {
                        while (reader.Read())
                        {
                            UserList.Add(new Person((int)reader[0], reader[1].ToString(), reader[2].ToString()));

                        }
                    }
                }
                conn.Close();

                watch.Stop();
                LogBlock += $"Finito {watch.ElapsedMilliseconds}";
            });
            th.Start();

        }

        private void ClearRelay(object obj)
        {

            lock (_syncLock)
            {
                UserList.Clear();
            }
            lock (_syncLockNotCollection)
            {
                UserListNotCollection.Clear();
            }
            LogBlock = "";
            Collection.Refresh();

        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private void AddMoreRelay(object obj)
        {
            UserList.Clear();

            //Console.WriteLine("Button Clicked");
            //MySqlCommand cmd;
            //conn.Open();
            //for (int j = 0; j < 100; j++)
            //{
            //    var cmdtext = $"INSERT INTO userlist(FirstName, LastName) VALUES ('{RandomString(10)}', '{RandomString(10)}'),";
            //    for (int i = 0; i < 500; i++)
            //    {
            //        cmdtext += $"('{RandomString(10)}', '{RandomString(10)}')";
            //        if (i != 499)
            //        {
            //            cmdtext += ",";
            //        }

            //    }

            //    cmd = new MySqlCommand(cmdtext, conn);
            //    Console.WriteLine($"Added {j}");
            //    cmd.ExecuteNonQuery();

            //}
            //conn.Close();

        }
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
