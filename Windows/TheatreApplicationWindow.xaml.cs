using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.ComponentModel;
using TheatreManagerApplication.Classes;
using TheatreManagerApplication.Backend;
using TheatreManagerApplication.Classes.ToFile;
using System.Data.SQLite;

namespace TheatreManagerApplication.Windows

{
    /// <summary>
    /// Interaction logic for TheatreApplicationWindow.xaml
    /// </summary>
    public partial class TheatreApplicationWindow : Window
    {
        public Theatre theatre;
        
        public TheatreApplicationWindow()
        {
            InitializeComponent();
            theatre = new Theatre();
            DatabaseManager.DatabaseManager.Initialise();
            InitializeSeatsArea(); //also fills in the Seats list and Row Dictionaries from DB
            InstantiateTheatre(); //fills in the lists from the DB
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //InitializeSeatsArea(); //also fills in the Seats list and Row Dictionaries from DB
            //InstantiateTheatre(); //fills in the lists from the DB
        }

        #region BookingTab
        private void InstantiateTheatre()
        {
            //ORDER OF INSTANTIATION IS IMPORTANT BECAUSE OF DEPENDENCIES
            theatre.InstantiateListOfAllCustomers();
            //theatre.InstantiateListOfAllPlays();
            //theatre.InstantiateListOfAllBookings();
        }

        #region IsMemberCheckboxBehaviour
        public string EmailBox = "";
        public string customerName = "";
        private void BookingTabIsMemberCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            BookingTabIsMemberCheckbox.IsChecked = false;
            BookingTabIsMemberCheckboxHandler(sender as CheckBox);
            BookingTabCustomerNameTextBox.Text = "";
            EmailBox = "";
            BookingTabEmailTextBox.Text = "";
            customerName = "";
        }

        private void BookingTabIsMemberCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            BookingTabIsMemberCheckbox.IsChecked = false;
            BookingTabIsMemberCheckboxHandler(sender as CheckBox);
            BookingTabCustomerNameTextBox.Text = customerName;
            BookingTabEmailTextBox.Text = EmailBox;
        }
        private void BookingTabCustomerNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            BookingTabCustomerNameTextBox.Text = "";
        }
        private void BookingTabEmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            BookingTabEmailTextBox.Text = "";
        }

        private void BookingTabIsMemberCheckboxHandler(CheckBox checkBox)
        {
            BookingTabCustomerNameTextBox.Text = "";
            BookingTabEmailTextBox.Text= "";
            bool isChecked = (bool)checkBox.IsChecked;
            if (isChecked)
            {
                //collapse
                BookingTabEmailTextBlock.Visibility = Visibility.Collapsed;
                BookingTabEmailTextBox.Visibility = Visibility.Collapsed;
                BookingTabAddAsMemberButton.Visibility = Visibility.Collapsed;
                //show
                BookingTabSearchGoldClubMembersListBox.Visibility = Visibility.Visible;
            }
            else
            {
                //show
                BookingTabEmailTextBlock.Visibility = Visibility.Visible;
                BookingTabEmailTextBox.Visibility = Visibility.Visible;
                BookingTabAddAsMemberButton.Visibility = Visibility.Visible;
                //colapse
                BookingTabSearchGoldClubMembersListBox.Visibility = Visibility.Collapsed;
            }
        }
        #endregion IsMemberCheckboxBehaviour

        #region SearchingGCMembersByName
        private void BookingTabCustomerNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(theatre != null)
            {
            string nameToSearchFor = BookingTabCustomerNameTextBox.Text;
            List<Customer> listOfCustomersThatMatchTheName = new List<Customer>();
            BookingTabSearchGoldClubMembersListBox.Items.Clear();

                if (nameToSearchFor != "")
                {
                    ListBoxItem memberItem = null;
                    listOfCustomersThatMatchTheName = Manager.Customer_Search(nameToSearchFor, "").ToList();//theatre.SearchListOfAllCustomersByName(nameToSearchFor);

                    if (listOfCustomersThatMatchTheName.Count == 0)
                    {
                        memberItem = new ListBoxItem();
                        memberItem.Content = "No GoldClub Member has name that matches'" + nameToSearchFor + "'.";
                        memberItem.Tag = "No results.";
                        BookingTabSearchGoldClubMembersListBox.Items.Add(memberItem);
                    }
                    else
                    {
                        foreach (Customer customer in listOfCustomersThatMatchTheName)
                        {
                            if (true)//customer.IsGoldClubMember())
                            {
                                memberItem = new ListBoxItem();
                                memberItem.Content = customer.ToString()+"\nIs Customer Gold Club Member: "+customer.IsGoldClubMember().ToString();
                                memberItem.Tag = customer.GetCustomerID().ToString();
                                memberItem.MouseDoubleClick += BookingTabSearchGoldClubMembersListBoxItem_MouseDoubleClick;
                                BookingTabSearchGoldClubMembersListBox.Items.Add(memberItem);
                            }
                        }
                    }
                    
                }
                else
                {
                   
                }
            
            }
        }

        private void BookingTabSearchGoldClubMembersListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = (ListBoxItem)sender;
            string customerIdString = (string)listBoxItem.Tag;
            int customerID;
            try
            {
                customerID = int.Parse(customerIdString);
            }
            catch
            {
                MessageBox.Show("Could not parse customer ID in BookingTabSearchGoldClubMembersListBoxItem_MouseDoubleClick",
                    "Parsing Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Customer selectedGoldClubMember = theatre.SearchListOfAllCustomersByID(customerID);

                if (true)
                {
                    customerName = selectedGoldClubMember.GetName();
                    EmailBox = selectedGoldClubMember.GetEmail();
                    BookingTabIsMemberCheckbox.IsChecked = false;
                    BookingTabIsMemberCheckboxHandler(BookingTabIsMemberCheckbox);
                    BookingTabIsMemberCheckbox.IsChecked = false;
                    BookingTabCustomerNameTextBox.Text = customerName;
                    BookingTabEmailTextBox.Text = EmailBox;
                    IsMember.IsChecked = false;

                }
            }
            catch
            { }

            }
        #endregion SearchingGCMembersByName

        private void BookingTabAddAsMemberButton_Click(object sender, RoutedEventArgs e)
        {
            string customerName = BookingTabCustomerNameTextBox.Text;
            string customerEmail = BookingTabEmailTextBox.Text;
           
            List<Play> emptyList = new List<Play>();
            DateTime startOfMembership = DateTime.UtcNow;
            //Manager.Customer_Search
            int customerIDCheck = Manager.Customer_GetId(customerName, customerEmail);
            if(customerEmail == "" || customerName == "")
            {
                MessageBox.Show("Name and Email Must Be Filled!");
                return;
            }
            if (customerIDCheck != -1)
            {
                MessageBox.Show("There is Already A Client Like This!");
                return;
            }

            Manager.Customer_Create(customerName, customerEmail, BookingTabIsMemberCheckbox.IsChecked.Value);



            int customerID; 
            
            customerID = Manager.Customer_GetId(customerName, customerEmail);

            theatre.AddCustomerToTheListOfAllCustomers(
                new Customer(customerName, customerEmail, customerID, BookingTabIsMemberCheckbox.IsChecked.Value)); //emptyList, startOfMembership, startOfMembership));
            MessageBox.Show("You added a Member");
        }

        private void BookingTabDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //refresh the seats view based on the DATE PICKED
        }

        private void BookingTabDatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            
            BookingTabPerformanceListBox.Items.Clear();
            if(!BookingTabDatePicker.SelectedDate.HasValue)
            {
                return;
            }
            DateTime date = (DateTime)BookingTabDatePicker.SelectedDate;
            if(date < DateTime.Now)
            {
                MessageBox.Show("You can not select a Date from the Past!");
                return;
            }
           if(date == null)
            {
                MessageBox.Show("Date Invalid!");
                return;
            }
            List<Performance> performances = new List<Performance>(); 
            //Here we acces the Database
            string sql = string.Format("SELECT * FROM Performances WHERE Date like '{0}%';", BookingTabDatePicker.SelectedDate.Value.ToShortDateString());
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);

            while (reader.Read())
            {
                performances.Add(new Performance((int)(Int64)reader["Performance_id"], (DateTime)reader["Date"], (int)(Int64)reader["Play_id"]));
            }
            //Here we have a list of performances with right date
            foreach (Performance performance in performances)
            {
                    Play play = Manager.Play_Get(performance.GetPlayIDFromPerformance()); //get play id from performance
                    ListBoxItem memberItem = null;
                    memberItem = new ListBoxItem();
                    memberItem.Content = play.GetPlayName().ToString() + " " + performance.GetPerformanceDate().ToString();
                    memberItem.Tag = performance.GetPerformanceID();
                    BookingTabPerformanceListBox.Items.Add(memberItem);
            }
        }
        private void BookingTabPerformanceListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ResetAllSeats();
            UIElement elem = (UIElement)BookingTabPerformanceListBox.InputHitTest(e.GetPosition(BookingTabPerformanceListBox));
            while (elem != BookingTabPerformanceListBox)
            {
                if (elem is ListBoxItem)
                {
                    //MessageBox.Show("MERGE");
                    ListBoxItem listBoxItem = (ListBoxItem)elem;
                    int performanceID = (int)listBoxItem.Tag;

                    List<Seat> seatsOcupied = new List<Seat>();
                    //Will be filled in here by a Joint Querry by Matei
                     string sql = string.Format("SELECT * FROM Seats Where seat_ID in (SELECT seat_id FROM[Reserved Seats] INNER JOIN Bookings ON Bookings.Booking_ID=[Reserved Seats].Booking_id AND[Reserved Seats].Performance_id = {0} AND Bookings.[Booking_state] != 1);", performanceID);
                    SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
                    while (reader.Read())
                    {
                        seatsOcupied.Add(new Seat(
                            (int)(Int64)reader["seat_ID"],
                            ((string)reader["seatRowCol"])[1],
                            ((string)reader["seatRowCol"])[2]+ ((string)reader["seatRowCol"])[3],
                            (SeatType)(int)(Int64)((string)reader["seatRowCol"])[0],
                            (bool)reader["isUsable"]));
                    }
                    //Here we have a list of seats ocupied
                        
                    foreach (Seat seat in seatsOcupied)
                    {
                        string seatButtonNameThatCorrespondsToListBoxItem = "SeatButton" + seat.GetSeatID().ToString();
                        Button button = FindChild<Button>(Application.Current.MainWindow, seatButtonNameThatCorrespondsToListBoxItem);
                        button.Background = Brushes.Black;
                        button.IsEnabled = false;
                    }
                   return;
                }
                
                elem = (UIElement)VisualTreeHelper.GetParent(elem);
            }
        }
        public void ResetAllSeats()
        {
            List<Seat> allSeats = new List<Seat>();
            string sql = "SELECT * FROM Seats";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            while (reader.Read())
            {
                allSeats.Add(new Seat(
                    (int)(Int64)reader["seat_ID"],
                    ((string)reader["seatRowCol"])[1],
                    ((string)reader["seatRowCol"])[2] + ((string)reader["seatRowCol"])[3],
                    Seat.GetSeatTypeFromString( ((string)reader["seatRowCol"])),
                    (bool)reader["isUsable"]));
            }
            foreach (Seat seat in allSeats)
            {
                string seatButtonNameThatCorrespondsToListBoxItem = "SeatButton" + seat.GetSeatID().ToString();
                Button button = FindChild<Button>(Application.Current.MainWindow, seatButtonNameThatCorrespondsToListBoxItem);
                button.Background = GetBrushColourBasedOnSeatType(seat.GetSeatType());
                button.IsEnabled = seat.IsUsable();
            }
        }
        #region SeatAreaGeneration
        private void InitializeSeatsArea()
        {
            theatre.InstantiateListOfAllSeats();
            theatre.InstantiateRowLengthDictionaries();

            List<Seat> listOfAllSeats = theatre.GetListOfAllSeats();
            Dictionary<char, int> dressRowLengthsDictionary = theatre.GetDressRowLengthDictionary();
            Dictionary<char, int> upperRowLengthsDictionary = theatre.GetUpperRowLengthDictionary();
            Dictionary<char, int> stallsRowLengthsDictionary = theatre.GetStallsRowLengthDictionary();
            int nextSeatPointer = listOfAllSeats.Count; //goes from N to 0 because of the mismatch between DB and the way of drawing in WPF


            BookingTabSeatAreaStackPanel.Children.Add(
                DrawArea(ref nextSeatPointer, listOfAllSeats, upperRowLengthsDictionary, "Upper Circle", GetBrushColourBasedOnSeatType(SeatType.Upper)));
            BookingTabSeatAreaStackPanel.Children.Add(
                DrawArea(ref nextSeatPointer, listOfAllSeats, dressRowLengthsDictionary, "Dress Circle", GetBrushColourBasedOnSeatType(SeatType.Dress)));
            BookingTabSeatAreaStackPanel.Children.Add(
                DrawArea(ref nextSeatPointer, listOfAllSeats, stallsRowLengthsDictionary, "Stalls", GetBrushColourBasedOnSeatType(SeatType.Stall)));
        }

        private UIElement DrawArea(ref int nextSeatPointer, List<Seat> listOfAllSeats,
            Dictionary<char, int> rowLengthDictionary, string areaName, Brush backgroundBrush)
        {
            StackPanel areaStackPanel = new StackPanel();
            TextBlock areaTextBlock = new TextBlock();

            areaTextBlock.Text = areaName;
            areaTextBlock.FontSize = 18;
            areaTextBlock.FontWeight = FontWeights.SemiBold;
            areaTextBlock.HorizontalAlignment = HorizontalAlignment.Center;

            //areaStackPanel.Name = "BookingTab" + areaName + "StackPanel";

            areaStackPanel.Children.Add(areaTextBlock);
            foreach (KeyValuePair<char, int> row in rowLengthDictionary)
            {
                areaStackPanel.Children.Add(DrawRowOfSeats(ref nextSeatPointer, listOfAllSeats, backgroundBrush, row.Key, row.Value));
            }

            return areaStackPanel;
        }

        private UIElement DrawRowOfSeats(ref int nextSeatPointer, List<Seat> listOfAllSeats, Brush backgroundBrush, char currentRow, int currentRowLength)
        {
            StackPanel rowStackPanel = new StackPanel();
            TextBlock startRowLetter = (TextBlock)RowLetter(currentRow);
            TextBlock endRowLetter = (TextBlock)RowLetter(currentRow);
            int seatIdFromSeatPointer = nextSeatPointer; //so that I can use it in lambda function of List.Find()
            List<Seat> listOfSeats = theatre.GetListOfAllSeats();
            Seat seatToDraw;

            rowStackPanel.Orientation = Orientation.Horizontal;
            rowStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            rowStackPanel.Margin = new Thickness(0, 0, 0, 0);

            //add letter to the beginning
            rowStackPanel.Children.Add(startRowLetter);

            for (int i = 1; i <= currentRowLength; i++)
            {
                seatToDraw = listOfSeats.Find(seat => seat.GetSeatID() == seatIdFromSeatPointer); //C# magic
                rowStackPanel.Children.Add(DrawSeat(seatToDraw, backgroundBrush, 30));
                if (seatIdFromSeatPointer == 2)
                {
                    nextSeatPointer--;
                    seatIdFromSeatPointer--;
                    seatToDraw = listOfSeats.Find(seat => seat.GetSeatID() == seatIdFromSeatPointer); //C# magic
                    rowStackPanel.Children.Add(DrawSeat(seatToDraw, backgroundBrush, 30));
                }
                nextSeatPointer--;
                seatIdFromSeatPointer--;
            }

            //add letter to the end
            rowStackPanel.Children.Add(endRowLetter);

            return rowStackPanel;
        }
        private UIElement DrawSeat(Seat seat, Brush backgroundBrush, int seatButtonSize)
        {
            Button seatButton = new Button();
            //RotateTransform rotateTransform = new RotateTransform(2, 0, 0);

            seatButton.Content = seat.GetSeatNumber().ToString();
            seatButton.Name = "SeatButton" + seat.GetSeatID().ToString();
            seatButton.Tag = seat.GetSeatID();
            seatButton.Width = seatButtonSize;
            seatButton.Height = seatButtonSize;
            seatButton.VerticalAlignment = VerticalAlignment.Top;
            seatButton.Margin = new Thickness(1, 0, 1, 0);
            seatButton.HorizontalAlignment = HorizontalAlignment.Left;
            seatButton.Background = backgroundBrush;
            //seatButton.RenderTransform = rotateTransform;

            if (seat.IsUsable())
            {
                seatButton.Click += new RoutedEventHandler(SelectSeat);
            }
            else
            {
                seatButton.IsEnabled = false;
                seatButton.Background = Brushes.DarkSlateGray;
            }
            return seatButton;
        }

        private UIElement RowLetter(char rowLetter)
        {
            TextBlock RowLetter = new TextBlock();
            RowLetter.Margin = new Thickness(10, 5, 10, 0);
            RowLetter.FontSize = 15;
            RowLetter.Height = 30;
            RowLetter.Width = 15;
            RowLetter.Text = (rowLetter.ToString());
            return RowLetter;
        }

        public Brush GetBrushColourBasedOnSeatType(SeatType seatType)
        {
            switch (seatType)
            {
                case SeatType.Upper:
                    return Brushes.Red;
                case SeatType.Dress:
                    return Brushes.Yellow;
                case SeatType.Stall:
                    return Brushes.Cyan;
                default:
                    return Brushes.GreenYellow; //something crazy to alert us
            }
        }

        private void SelectSeat(object sender, RoutedEventArgs e)
        {
            Button selected = (Button)sender;
            ListBoxItem seatItem = new ListBoxItem();
            Seat selectedSeat;
            if((ListBoxItem)BookingTabPerformanceListBox.SelectedItem == null)
            {
                MessageBox.Show("A Play must be selected");
                return;
            }
            selected.Background = Brushes.DarkSlateGray;
            selectedSeat = theatre.GetListOfAllSeats().Find(seat => seat.GetSeatID() == (int)selected.Tag);

            ListBoxItem performanceItem = (ListBoxItem)BookingTabPerformanceListBox.SelectedItem;

            Performance performance = Manager.Performances_Get((int)performanceItem.Tag);
            Play play = Manager.Play_Get(performance.GetPlayIDFromPerformance());

            float price = play.GetBasePriceForSeat();
            if(selectedSeat.GetSeatType() == SeatType.Stall)
            {
                price += 10;
            }
            if(selectedSeat.GetSeatType() == SeatType.Dress)
            {
                price += 15;
            }
            if(selectedSeat.GetSeatType() == SeatType.Upper)
            {
                price += 13;
            }
            int count = 0;
            var items = FindVisualChildren<ListBoxItem>(BookingTabSelectedSeatsListBox1).ToList();
            foreach (ListBoxItem i in items)
            {
                int id = (int)i.Tag;
                Seat seatThatCorrespondsToListBoxItem = theatre.GetListOfAllSeats().Find(seat => seat.GetSeatID() == id);
                count++;
            }
            if(count > 6)
            {
                MessageBox.Show("You can only select 6 seats");
                return;
            }

            seatItem.Content = "Number " + selectedSeat.GetSeatNumber().ToString() + " Row:  " + selectedSeat.GetSeatRow() + " | " + "Price: "+price.ToString();
            selectedSeat.SetSeatPrice(price);// Maybe, hope it wokrs
            seatItem.Tag = selectedSeat.GetSeatID();
            seatItem.Background = Brushes.PeachPuff;
            BookingTabSelectedSeatsListBox.Items.Add(seatItem);
            MessageBox.Show(seatItem.Content.ToString()); //for testing
            BookingTabPriceTextBox.Text = CalculateTotalPrice();
            BookingTabPriceTextBox.Text = CalculateTotalPrice();

        }
        #endregion SeatAreaGeneration
        private string CalculateTotalPrice()
        {
            float total = 0f;
            var items = FindVisualChildren<ListBoxItem>(BookingTabSelectedSeatsListBox).ToList();
            foreach (ListBoxItem i in items)
            {
                int id = (int)i.Tag;
                Seat seatThatCorrespondsToListBoxItem = theatre.GetListOfAllSeats().Find(seat => seat.GetSeatID() == id);
                total += seatThatCorrespondsToListBoxItem.GetSeatPrice();
            }
            try { 
            if (Manager.Customer_Get(Manager.Customer_GetId(BookingTabCustomerNameTextBox.Text, BookingTabEmailTextBox.Text)).IsGoldClubMember() == true)
            {
                total = total - total * 0.1f;
            }
            }
            catch { }
            total = (int)total;
            return total.ToString();
        }
        private void BookingTabDeleteSeatButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedItemTag = (int)((ListBoxItem)BookingTabSelectedSeatsListBox.SelectedItem).Tag;
                Seat seatThatCorrespondsToListBoxItem = theatre.GetListOfAllSeats().Find(seat => seat.GetSeatID() == selectedItemTag);
                string seatButtonNameThatCorrespondsToListBoxItem = "SeatButton" + seatThatCorrespondsToListBoxItem.GetSeatID().ToString();
                Button theButton = FindChild<Button>(Application.Current.MainWindow, seatButtonNameThatCorrespondsToListBoxItem);
                theButton.Background = GetBrushColourBasedOnSeatType(seatThatCorrespondsToListBoxItem.GetSeatType());
                BookingTabSelectedSeatsListBox.Items.Remove(BookingTabSelectedSeatsListBox.SelectedItem);
                MessageBox.Show("You Removed the seat from the booking");
                BookingTabPriceTextBox.Text = CalculateTotalPrice();
            }
            catch
            {
                MessageBox.Show("Please select an item first!");
            }
        }

        private void BookingTabCreateBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = BookingTabCustomerNameTextBox.Text;
                string email = BookingTabEmailTextBox.Text;
                Customer customer = null;
                try
                {
                    customer = Manager.Customer_Search(name, email)[0];
                }
                catch
                {
                    MessageBox.Show("You Must Complete More Fields!");
                    return;
                }

                Performance performance = Manager.Performances_Get((int)((ListBoxItem)BookingTabPerformanceListBox.SelectedItem).Tag);
                int bookingID = 0;
                int price = int.Parse(BookingTabPriceTextBox.Text);


                Manager.Bookings_Create(performance.GetPerformanceID(), customer.GetCustomerID(), 3, false, price);
                string sql = "SELECT * FROM Bookings WHERE Performance_id=" + performance.GetPerformanceID() + " AND Customer_id=" + customer.GetCustomerID() + ";";
                SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
                while (reader.Read())
                {
                    bookingID = (int)(Int64)reader["Booking_id"];
                }
                var items = FindVisualChildren<ListBoxItem>(BookingTabSelectedSeatsListBox).ToList();
                foreach (ListBoxItem i in items)
                {
                    int id = (int)i.Tag;
                    Seat seatThatCorrespondsToListBoxItem = theatre.GetListOfAllSeats().Find(seat => seat.GetSeatID() == id);
                    Manager.Seats_Reserve(bookingID, performance.GetPerformanceID(), seatThatCorrespondsToListBoxItem.GetSeatID());
                    string seatButtonNameThatCorrespondsToListBoxItem = "SeatButton" + seatThatCorrespondsToListBoxItem.GetSeatID().ToString();
                    Button button = FindChild<Button>(Application.Current.MainWindow, seatButtonNameThatCorrespondsToListBoxItem);
                    button.Background = GetBrushColourBasedOnSeatType(seatThatCorrespondsToListBoxItem.GetSeatType());
                }
                MessageBox.Show("You created  a booking");
                BookingTabDatePicker.SelectedDate = DateTime.Now;
                BookingTabPerformanceListBox.Items.Clear();
                BookingTabSelectedSeatsListBox.Items.Clear();
                BookingTabPriceTextBox.Text = "";
                ResetAllSeats();
            }
            catch
            {

            }



        }

        struct NEWBOOKING
        {
            public int BookingID;
            public bool IsConfirmed;
            public int price;
        }
        private void SearchBookin_Click(object sender, RoutedEventArgs e)
        {
            ListOfBookings.Items.Clear();
            string name = BookingTabCustomerNameTextBox1.Text;
            string email = BookingTabEmailTextBox1.Text;

            if(name == "" || email == "")
            {
                MessageBox.Show("Name and Email Must Be Filled!");
                return;
            }

            List<NEWBOOKING> bookings = new List<NEWBOOKING>();

            string sql = string.Format("SELECT Booking_id, Price, isPaid FROM Bookings INNER JOIN Customers ON Bookings.Customer_id=Customers.Customer_id AND Customers.Customer_name='{0}' AND Customers.Contact_email='{1}'", name, email);
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            while (reader.Read())
            {
                NEWBOOKING booking;
                booking.BookingID = (int)(Int64)reader["Booking_id"];
                booking.IsConfirmed = (bool)reader["isPaid"];
                booking.price = (int)(Int64)reader["Price"];
                bookings.Add(booking);
                
            }
            foreach(NEWBOOKING booking in bookings)
            {
                ListBoxItem memberItem = new ListBoxItem();
                memberItem.Content = "Number of Booking: " + booking.BookingID;
                memberItem.Tag = booking.BookingID;
                ListOfBookings.Items.Add(memberItem);

            }

           
        }

        private void ListOfBookings_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BookingTabSelectedSeatsListBox1.Items.Clear();
            UIElement elem = (UIElement)ListOfBookings.InputHitTest(e.GetPosition(ListOfBookings));
            while (elem != ListOfBookings)
            {
                if (elem is ListBoxItem)
                {
                    
                    ListBoxItem listBoxItem = (ListBoxItem)elem;
                    int BookingID = (int)listBoxItem.Tag;

                    string sql = string.Format("SELECT * FROM Seats Where seat_ID in (SELECT seat_id FROM [Reserved Seats] WHERE Booking_id={0})", BookingID);
                    SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
                    while (reader.Read())
                    {
                        Seat seatsOcupied = new Seat(
                            (int)(Int64)reader["seat_ID"],
                            ((string)reader["seatRowCol"])[1],
                            ((string)reader["seatRowCol"])[2] + ((string)reader["seatRowCol"])[3],
                            (SeatType)(int)(Int64)((string)reader["seatRowCol"])[0],
                            (bool)reader["isUsable"]);
                        ListBoxItem memberItem = new ListBoxItem();
                        memberItem.Content = "SeatButton" + seatsOcupied.GetSeatID().ToString();
                        memberItem.Tag = seatsOcupied.GetSeatID();
                        BookingTabSelectedSeatsListBox1.Items.Add(memberItem);

                        // highlight buttons
                        Button selectedButton = FindChild<Button>(Application.Current.MainWindow, memberItem.Content.ToString());
                        selectedButton.Background = Brushes.DarkSlateGray;
                    }

                    Booking booking = Manager.Bookings_Get(BookingID);

                    if (booking.IsPaid() == true)
                    {
                        PlaysTabMajorPlayRadioButton1.IsChecked = true;
                        PlaysTabMinorPlayRadioButton1.IsChecked = false;

                    }
                    if (booking.IsPaid() == false)
                    {
                        PlaysTabMajorPlayRadioButton1.IsChecked = false;
                        PlaysTabMinorPlayRadioButton1.IsChecked = true;
                    }
                   
                   

                }

                elem = (UIElement)VisualTreeHelper.GetParent(elem);
            }
        }

        private void BookingTabDeleteSeatButton1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedItemTag = (int)((ListBoxItem)BookingTabSelectedSeatsListBox1.SelectedItem).Tag;
                Seat seatThatCorrespondsToListBoxItem = theatre.GetListOfAllSeats().Find(seat => seat.GetSeatID() == selectedItemTag);
                string seatButtonNameThatCorrespondsToListBoxItem = "SeatButton" + seatThatCorrespondsToListBoxItem.GetSeatID().ToString();
                Button theButton = FindChild<Button>(Application.Current.MainWindow, seatButtonNameThatCorrespondsToListBoxItem);
                theButton.Background = GetBrushColourBasedOnSeatType(seatThatCorrespondsToListBoxItem.GetSeatType());
                BookingTabSelectedSeatsListBox1.Items.Remove(BookingTabSelectedSeatsListBox1.SelectedItem);
                MessageBox.Show("You Removed the seat from the booking");
                
            }
            catch
            {
                MessageBox.Show("Please select an item first!");
            }
        }

        // SAVE EDITED BOOKING
        private void BookingTabCreateBookingButton1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = BookingTabCustomerNameTextBox1.Text;
                string email = BookingTabEmailTextBox1.Text;
                Customer customer = null;
                try
                {
                    customer = new Customer(name, email, Manager.Customer_GetId(name, email), Manager.Customer_Get(Manager.Customer_GetId(name, email)).IsGoldClubMember());
                }
                catch
                {
                    MessageBox.Show("Select More Items First!");
                    return;
                }
                if (name == "" || email == "")
                {
                    MessageBox.Show("Name and Email Must Be Filled!");
                    return;
                }

                int bookingID = (int)((ListBoxItem)ListOfBookings.SelectedItem).Tag;
                Booking oldBooking = Manager.Bookings_Get(bookingID);

                List<Seat> seats = new List<Seat>();
                var items = FindVisualChildren<ListBoxItem>(BookingTabSelectedSeatsListBox1).ToList();
                foreach (ListBoxItem i in items)
                {
                    int id = (int)i.Tag;
                    Seat seatThatCorrespondsToListBoxItem = theatre.GetListOfAllSeats().Find(seat => seat.GetSeatID() == id);
                    seats.Add(seatThatCorrespondsToListBoxItem);
                }

                BookingState stateofbooking = BookingState.Active;
                bool isPaid = true;
                if (PlaysTabMajorPlayRadioButton1.IsChecked == true)
                {
                    stateofbooking = BookingState.Active;
                    isPaid = true;
                }
                if (PlaysTabMinorPlayRadioButton1.IsChecked == true)
                {
                    stateofbooking = BookingState.Pending;
                    isPaid = false;
                }
                int price = 0;
                foreach (Seat seat in seats)
                {
                    price += (int)seat.CalculateThisSeatPrice(10);
                }
                Booking booking = new Booking(bookingID, oldBooking.GetPerformanceThisBookingIsFor(), seats, stateofbooking, customer, isPaid, price);
                Manager.Bookings_Update(bookingID, booking);

                string sql = string.Format("DELETE FROM [Reserved Seats] WHERE Booking_id={0};", bookingID);
                DBManager.DoQuery_Write(sql);

                foreach (Seat seat in seats)
                {
                    Manager.Seats_Reserve(bookingID, booking.GetPerformanceThisBookingIsFor().GetPerformanceID(), seat.GetSeatID());

                }
                MessageBox.Show("You edited a Booking!");
                ListOfBookings.Items.Clear();
                BookingTabSelectedSeatsListBox1.Items.Clear();
                PlaysTabMajorPlayRadioButton1.IsChecked = false;
                PlaysTabMinorPlayRadioButton1.IsChecked = false;
                ResetAllSeats();
            }
            catch { }

        }
        #endregion BookingTab

        #region PlaysTab

        private void PlaysTabAddPlayButton_Click(object sender, RoutedEventArgs e)
        {
            bool isViable = true;
            string name = NameTag.Text;
            string author = AuthorTag.Text;

            if(name == "" || author == "")
            {
                MessageBox.Show("You Must Fill In The Name and Author!");
                return;
            }
            //string actors = ActorsTag.Text;
            int price = 0;
            try
            {
                price = int.Parse(BasePriceTag.Text);
            }
            catch
            {
                MessageBox.Show("Base Price Must Be A Number!");
                isViable = false;
            }

            DateTime startTime = DateTime.Now;
            try
            {
                startTime = PlaysTabFirstPerformanceDatePicker.SelectedDate.Value;
            }
            catch
            {
                startTime = DateTime.Now;
            }
            DateTime endTime = DateTime.Now;
            try
            {
                endTime = PlaysTabLastPerformanceDatePicker.SelectedDate.Value;
            }
            catch
            {
                endTime = DateTime.Now;
            }
            PlayType type = new PlayType();
            if (PlaysTabMajorPlayRadioButton.IsChecked.Value == true)
            {
                type = PlayType.Major;
            }
            if (PlaysTabMinorPlayRadioButton.IsChecked.Value == true)
            {
                type = PlayType.Minor;
            }
            if (PlaysTabOneTimePerformanceRadioButton.IsChecked.Value == true)
            {
                type = PlayType.OneTime;
            }
            if (PlaysTabMajorPlayRadioButton.IsChecked.Value == false && PlaysTabMinorPlayRadioButton.IsChecked.Value == false && PlaysTabOneTimePerformanceRadioButton.IsChecked.Value == false)
            {
                MessageBox.Show("You Must Choose a Type Of Play!");
                isViable = false;
            }
            if (endTime - startTime < new TimeSpan(0, 0, 0, 0, 0) && type != PlayType.OneTime)
            {
                MessageBox.Show("Start Time Must Be Before End Time");
                isViable = false;
            }
            if(startTime.Month != endTime.Month || startTime.Year != endTime.Year)
            {
                isViable = false;
                MessageBox.Show("A Play Must Have Perfrmances Only In One Month!");
            }

            if (isViable)
            {
                List<Performance> performances = new List<Performance>();
                if(Manager.Play_Create(name, author, price, performances, type) == DBResult.AlreadyExists)
                {
                    MessageBox.Show("Play Must Be Unique");
                    return;
                }
                int playID = Manager.Play_GetID(name, author);
                if (type == PlayType.Major)
                {
                    TimeSpan playrun = endTime - startTime;
                    int performanceID = 0;
                    for (int i = 0; i <= playrun.Days; i++)
                    {
                        var timetoaddperformance = startTime.AddDays(i);
                        if (timetoaddperformance.DayOfWeek == DayOfWeek.Monday)
                        {
                            performances.Add(new Performance(performanceID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)), playID));
                            Manager.Performance_Create(playID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)));
                            performanceID++;
                        }
                        if (timetoaddperformance.DayOfWeek == DayOfWeek.Thursday)
                        {
                            performances.Add(new Performance(performanceID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)), playID));
                            Manager.Performance_Create(playID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)));
                            performanceID++;
                        }
                        if (timetoaddperformance.DayOfWeek == DayOfWeek.Friday)
                        {
                            performances.Add(new Performance(performanceID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)), playID));
                            Manager.Performance_Create(playID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)));
                            performanceID++;
                        }
                        if (timetoaddperformance.DayOfWeek == DayOfWeek.Saturday)
                        {
                            performances.Add(new Performance(performanceID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)), playID));
                            Manager.Performance_Create(playID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)));
                            performanceID++;
                            performances.Add(new Performance(performanceID, timetoaddperformance.Add(new TimeSpan(13, 0, 0)), playID));
                            Manager.Performance_Create(playID, timetoaddperformance.Add(new TimeSpan(13, 0, 0)));
                            performanceID++;
                        }
                        if (timetoaddperformance.DayOfWeek == DayOfWeek.Sunday)
                        {
                            performances.Add(new Performance(performanceID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)), playID));
                            Manager.Performance_Create(playID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)));
                            performanceID++;
                        }

                    }
                }
                if (type == PlayType.Minor)
                {
                    TimeSpan playrun = endTime - startTime;
                    int performanceID = 0;
                    for (int i = 0; i <= playrun.Days; i++)
                    {
                        var timetoaddperformance = startTime.AddDays(i);
                        if (timetoaddperformance.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            performances.Add(new Performance(performanceID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)), playID));
                            Manager.Performance_Create(playID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)));
                            performanceID++;
                        }
                        if (timetoaddperformance.DayOfWeek == DayOfWeek.Wednesday)
                        {
                            performances.Add(new Performance(performanceID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)), playID));
                            Manager.Performance_Create(playID, timetoaddperformance.Add(new TimeSpan(20, 0, 0)));
                            performanceID++;
                        }
                    }
                }
                if (type == PlayType.OneTime)
                {
                    
                  int performanceID = 0;
                  performances.Add(new Performance(performanceID, startTime.Add(new TimeSpan(20, 0, 0)), playID));
                  Manager.Performance_Create(playID, startTime.Add(new TimeSpan(20, 0, 0)));
                  performanceID++;
                       
                    
                }
                Play play = new Play(playID, name, type, author, price, performances);
                theatre.AddPlayToTheListOfAllPlays(play);

                PlaysTabPlaysListBox.Items.Clear();
                ListBoxItem memberItem = null;
                memberItem = new ListBoxItem();
                memberItem.Content = "Name: " + play.GetPlayName() + " | " + "Author: " + play.GetAuthor() + " | " + "Price: " + play.GetBasePriceForSeat();
                memberItem.Tag = play.GetPlayID().ToString();
                PlaysTabPlaysListBox.Items.Add(memberItem);
                


            }
        }

        private void PlaysTabSearchPlayButton_Click(object sender, RoutedEventArgs e)
        {
            bool isViable = true;
            string name = NameTag.Text;
            string author = AuthorTag.Text;
            //string actors = ActorsTag.Text;
            bool isplayactive = PlaysTabShowActiveCheckBox.IsChecked.Value;
            bool isplayinactive = PlaysTabShowInactiveCheckBox.IsChecked.Value;
            PlaysTabPerformancesListBox.Items.Clear();

            if(name == "" || author == "")
            {
                MessageBox.Show("You must Fill the Name and Author Field To Search!");
                return;
            }
            

           

            if (isViable)
            {
                PlaysTabPlaysListBox.Items.Clear();
                List<Play> plays = new List<Play>();
                Play[] ArrayofPlays = Manager.Play_Search(name, author); //here we get the list of plays
                for(int i = 0; i < ArrayofPlays.Length; i++) //going rough the list of plays and geting the performances
                {
                    List<Performance> performances = Manager.Performances_Fetch(ArrayofPlays[i].GetPlayID());
                    for(int j = 0; j < performances.Count; j++)
                    {
                        ArrayofPlays[i].AddPerformanceToTheListOfPerformances(performances[j]);
                    }
                    plays.Add(ArrayofPlays[i]);
                }
                ListBoxItem memberItem = null;
                foreach (Play play in plays)
                {
                    if(isplayactive == true)
                        if (DateTime.Now < play.GetListOfPerformancesOfThisPlay()[play.GetListOfPerformancesOfThisPlay().Count - 1].GetPerformanceDate())
                        {
                            memberItem = new ListBoxItem();
                            memberItem.Content = "Name: " + play.GetPlayName() + " | " + "Author: " + play.GetAuthor() + " | " + "Price: " + play.GetBasePriceForSeat();
                            memberItem.Tag = play.GetPlayID().ToString();
                            PlaysTabPlaysListBox.Items.Add(memberItem);
                        }
                    if(isplayinactive == true)
                        if (DateTime.Now > play.GetListOfPerformancesOfThisPlay()[play.GetListOfPerformancesOfThisPlay().Count - 1].GetPerformanceDate())
                        {
                            memberItem = new ListBoxItem();
                            memberItem.Content = "Name: " + play.GetPlayName() + " | " + "Author: " + play.GetAuthor() + " | " + "Price: " + play.GetBasePriceForSeat();
                            memberItem.Tag = play.GetPlayID().ToString();
                            PlaysTabPlaysListBox.Items.Add(memberItem);
                        }                   
                }
                if(ArrayofPlays.Length == 0)
                {
                    memberItem = new ListBoxItem();
                    memberItem.Content = "No Play has name that matches'" + name + "'.";
                    memberItem.Tag = "No results.";
                    PlaysTabPlaysListBox.Items.Add(memberItem);
                }
            }

        }

        private void PlaysTabPlaysListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            UIElement elem = (UIElement)PlaysTabPlaysListBox.InputHitTest(e.GetPosition(PlaysTabPlaysListBox));
            while (elem != PlaysTabPlaysListBox)
            {
                if (elem is ListBoxItem)
                {
                    //object selectedItem = ((ListBoxItem)elem).Content;
                    PlaysTabPerformancesListBox.Items.Clear();
                    ListBoxItem listBoxItem = (ListBoxItem)elem;
                    string customerIdString = (string)listBoxItem.Tag;
                    PlaysTabPerformancesListBox.Items.Clear();
                    int playID;
                    try
                    {
                        playID = int.Parse(customerIdString);
                    }
                    catch
                    {
                        MessageBox.Show("Could not parse customer ID in BookingTabSearchGoldClubMembersListBoxItem_MouseDoubleClick",
                            "Parsing Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    Play selectedPlay = Manager.Play_Get(playID);
                    
                    foreach (Performance performance in Manager.Performances_Fetch(selectedPlay.GetPlayID()))
                    {
                        ListBoxItem memberItem = null;
                        memberItem = new ListBoxItem();
                        memberItem.Content = performance.GetPerformanceDate();
                        memberItem.Tag = performance.GetPerformanceID();
                        PlaysTabPerformancesListBox.Items.Add(memberItem);
                    }

                    NameTag.Text = selectedPlay.GetPlayName();
                    AuthorTag.Text = selectedPlay.GetAuthor();
                    BasePriceTag.Text =  selectedPlay.GetBasePriceForSeat().ToString();
                    PlaysTabFirstPerformanceDatePicker.SelectedDate = Manager.Performances_Fetch(selectedPlay.GetPlayID())[0].GetPerformanceDate();
                    PlaysTabLastPerformanceDatePicker.SelectedDate = Manager.Performances_Fetch(selectedPlay.GetPlayID())[Manager.Performances_Fetch(selectedPlay.GetPlayID()).Count-1].GetPerformanceDate();
                    if(selectedPlay.GetPlayType() == PlayType.Major)
                    {
                        PlaysTabMajorPlayRadioButton.IsChecked = true;
                        PlaysTabMinorPlayRadioButton.IsChecked = false;
                        PlaysTabOneTimePerformanceRadioButton.IsChecked = false;
                    }
                    if (selectedPlay.GetPlayType() == PlayType.Minor)
                    {
                        PlaysTabMajorPlayRadioButton.IsChecked = false;
                        PlaysTabMinorPlayRadioButton.IsChecked = true;
                        PlaysTabOneTimePerformanceRadioButton.IsChecked = false;
                    }
                    if (selectedPlay.GetPlayType() == PlayType.OneTime)
                    {
                        PlaysTabMajorPlayRadioButton.IsChecked = false;
                        PlaysTabMinorPlayRadioButton.IsChecked = false;
                        PlaysTabOneTimePerformanceRadioButton.IsChecked = true;
                    }
                    return;
                }
                elem = (UIElement)VisualTreeHelper.GetParent(elem);
            }
            
        }

        private void PlaysTabEditPlayButton_Click(object sender, RoutedEventArgs e)
        {
            bool isViable = true;
            string name = NameTag.Text;
            string author = AuthorTag.Text;
            int price = 0;
            try
            {
                price = int.Parse(BasePriceTag.Text);
            }
            catch
            {
                MessageBox.Show("Base Price Must Be A Number!");
                isViable = false;
            }

            DateTime startTime = DateTime.Now;
            try
            {
                startTime = PlaysTabFirstPerformanceDatePicker.SelectedDate.Value;
            }
            catch
            {
                startTime = DateTime.Now;
            }
            DateTime endTime = DateTime.Now;
            try
            {
                endTime = PlaysTabLastPerformanceDatePicker.SelectedDate.Value;
            }
            catch
            {
                endTime = DateTime.Now;
            }
            if (endTime - startTime < new TimeSpan(0, 0, 0, 0, 0))
            {
                MessageBox.Show("Start Time Must Be Before End Time");
                isViable = false;
            }
            PlayType type = new PlayType();
            if (PlaysTabMajorPlayRadioButton.IsChecked.Value == true)
            {
                type = PlayType.Major;
            }
            if (PlaysTabMinorPlayRadioButton.IsChecked.Value == true)
            {
                type = PlayType.Minor;
            }
            if (PlaysTabOneTimePerformanceRadioButton.IsChecked.Value == true)
            {
                type = PlayType.OneTime;
            }
            if (PlaysTabMajorPlayRadioButton.IsChecked.Value == false && PlaysTabMinorPlayRadioButton.IsChecked.Value == false && PlaysTabOneTimePerformanceRadioButton.IsChecked.Value == false)
            {
                MessageBox.Show("You Must Choose a Type Of Play!");
                isViable = false;
            }
            if (isViable)
            {
                Play play;
                int playID = 0;
                try
                {
                    playID = Manager.Play_GetID(name, author);
                }
                catch
                {
                    MessageBox.Show("There is no Play Like This!");
                    return;
                }
                if(playID == -1)
                {
                    MessageBox.Show("There is no Play Like This! or You Are Trying to Edit The Name Or Author Of The Play");
                    return;
                }
                play = Manager.Play_Get(playID);

                Manager.Play_Update(playID, name, author, price);
               
                
            }
        }

        private void PlaysTabDeletePlayButton_Click(object sender, RoutedEventArgs e)
        {
            bool isViable = true;
            string name = NameTag.Text;
            string author = AuthorTag.Text;
            int price = 0;
            try
            {
                price = int.Parse(BasePriceTag.Text);
            }
            catch
            {
                MessageBox.Show("Base Price Must Be A Number!");
                isViable = false;
            }

            DateTime startTime = DateTime.Now;
            try
            {
                startTime = PlaysTabFirstPerformanceDatePicker.SelectedDate.Value;
            }
            catch
            {
                startTime = DateTime.Now;
            }
            DateTime endTime = DateTime.Now;
            try
            {
                endTime = PlaysTabLastPerformanceDatePicker.SelectedDate.Value;
            }
            catch
            {
                endTime = DateTime.Now;
            }
            if (endTime - startTime < new TimeSpan(0, 0, 0, 0, 0))
            {
                MessageBox.Show("Start Time Must Be Before End Time");
                isViable = false;
            }
            PlayType type = new PlayType();
            if (PlaysTabMajorPlayRadioButton.IsChecked.Value == true)
            {
                type = PlayType.Major;
            }
            if (PlaysTabMinorPlayRadioButton.IsChecked.Value == true)
            {
                type = PlayType.Minor;
            }
            if (PlaysTabOneTimePerformanceRadioButton.IsChecked.Value == true)
            {
                type = PlayType.OneTime;
            }
            if (PlaysTabMajorPlayRadioButton.IsChecked.Value == false && PlaysTabMinorPlayRadioButton.IsChecked.Value == false && PlaysTabOneTimePerformanceRadioButton.IsChecked.Value == false)
            {
                MessageBox.Show("You Must Choose a Type Of Play!");
                isViable = false;
            }
            if (isViable)
            {
                Play play;
                int playID = 0;
                try
                {
                    playID = Manager.Play_GetID(name, author);
                }
                catch
                {
                    MessageBox.Show("There is no Play Like This!");
                    return;
                }
                if (playID == -1)
                {
                    MessageBox.Show("The Name of The Author and The Play Must Be Right");
                    return;
                }
                play = Manager.Play_Get(playID);

                Manager.Play_Delete(playID);
                PlaysTabPerformancesListBox.Items.Clear();
                PlaysTabPlaysListBox.Items.Clear();

            }
        }

        private void PlaysTabOneTimePerformanceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            PlaysTabLastPerformanceDateTextBlock.Visibility = Visibility.Collapsed;
            PlaysTabLastPerformanceDatePicker.Visibility = Visibility.Collapsed;
        }

        private void PlaysTabOneTimePerformanceRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            PlaysTabLastPerformanceDateTextBlock.Visibility = Visibility.Visible;
            PlaysTabLastPerformanceDatePicker.Visibility = Visibility.Visible;
        }

        private void PlaysTabShowActiveCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PlaysTabShowActiveCheckboxHandler(sender as CheckBox);
        }
        private void PlaysTabShowActiveCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PlaysTabShowActiveCheckboxHandler(sender as CheckBox);
        }

        private void PlaysTabShowActiveCheckboxHandler(CheckBox checkBox)
        {
            bool isChecked = (bool)checkBox.IsChecked;
            if (isChecked)
            {
                //Make relevant input fields visible and vice versa
            }
            else
            {
                //Make relevant input fields visible and vice versa
            }
        }

        private void PlaysTabShowInactiveCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PlaysTabShowInactiveCheckboxHandler(sender as CheckBox);
        }

        private void PlaysTabShowInactiveCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PlaysTabShowInactiveCheckboxHandler(sender as CheckBox);
        }
        private void PlaysTabShowInactiveCheckboxHandler(CheckBox checkBox)
        {
            bool isChecked = (bool)checkBox.IsChecked;
            if (isChecked)
            {
                //Make relevant input fields visible and vice versa
            }
            else
            {
                //Make relevant input fields visible and vice versa
            }
        }

        private void NameTag_GotFocus(object sender, RoutedEventArgs e)
        {
            NameTag.Text = "";
        }

        private void AuthorTag_GotFocus(object sender, RoutedEventArgs e)
        {
            AuthorTag.Text = "";
        }

        private void ActorsTag_GotFocus(object sender, RoutedEventArgs e)
        {
            //ActorsTag.Text = "";
        }

        private void BasePriceTag_GotFocus(object sender, RoutedEventArgs e)
        {
            BasePriceTag.Text = "";
        }
        #endregion PlaysTab

        #region ManagerTAB

        private void ManagerTabDeleteCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            string name = ManagerTabDeleteCustomerName.Text;
            string email = ManagerTabDeleteCustomerEmail.Text;

            if (name == "" || email == "")
            {
                MessageBox.Show("Both The Name And The Email Must Be Filled");
                return;
            }
            if (Manager.Customer_GetId(name, email) == -1)
            {
                MessageBox.Show("There Is No Such Customer!");
                return;
            }
            Manager.Customer_Delete(Manager.Customer_GetId(name, email));
            MessageBox.Show("Customer Deleted!");
        }

        private void ManagerTabGoldClubButton_Click(object sender, RoutedEventArgs e)
        {
            string name = ManagerTabGoldClubName.Text;
            string email = ManagerTabGoldClubEmail.Text;

            if (name == "" || email == "")
            {
                MessageBox.Show("Both The Name And The Email Must Be Filled");
                return;
            }
            if (Manager.Customer_GetId(name, email) == -1)
            {
                MessageBox.Show("There Is No Such Customer!");
                return;
            }
            int id = Manager.Customer_GetId(name, email);
            Customer customer = Manager.Customer_Get(id);
            if (customer.IsGoldClubMember() == false)
            {
                Manager.Customer_Delete(Manager.Customer_GetId(name, email));

                Manager.Customer_Create(name, email, true);

                MessageBox.Show("You made" + customer.ToString() + "Into a Gold Club Member");
                return;
            }
            else
            {
                MessageBox.Show("The Customer Is already Part Of Gold Club Member!");
                return;
            }
            
        }

        private void CreateSaleReport_Click(object sender, RoutedEventArgs e)
        {
            ToFile.MakeSalesReport(Year.Text);
            MessageBox.Show("You Created a report with the Sales in the year that you selected(Default 2016)");
        }

        private void ManagerTabDeleteCustomerName_GotFocus(object sender, RoutedEventArgs e)
        {
            ManagerTabDeleteCustomerName.Text = "";
        }

        private void ManagerTabDeleteCustomerEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            ManagerTabDeleteCustomerEmail.Text = "";
        }

        private void ManagerTabGoldClubName_GotFocus(object sender, RoutedEventArgs e)
        {
            ManagerTabGoldClubName.Text = "";
        }

        private void ManagerTabGoldClubEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            ManagerTabGoldClubEmail.Text = "";
        }

        private void CreateListOfGoldClubMembers_Click(object sender, RoutedEventArgs e)
        {
            string[] labels = new string[2];
            labels[0] = "Name";
            labels[1] = "Email";

            List<Customer> customers = new List<Customer>();

            string sql = "SELECT * FROM Customers WHERE isGoldClubMember = 'True';"; //WHERE isGoldClubMember = 'True'
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            while (reader.Read()) {
                customers.Add(new Customer((string)reader["Customer_name"], (string)reader["Contact_email"], (int)(Int64)reader["Customer_id"], (bool)reader["isGoldClubMember"]));
            }
            string[,] info = new string[customers.Count, 2];
            for(int i = 0; i < customers.Count; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    if(j == 0)
                    {
                        info[i,j] = customers[i].GetName();
                    }
                    if(j == 1)
                    {
                        info[i, j] = customers[i].GetEmail();
                    }
                }
            }
            ToFile.WriteToFile(labels, info, customers.Count, 2, "ListOfGoldClubMembers");
            MessageBox.Show("You Created a Report with all the Gold Club Members and their Email");
        }

        private void GoldClubTabShowActiveCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            GoldClubTabShowActiveCheckboxHandler(sender as CheckBox);
        }

        private void GoldClubTabShowInactiveCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            GoldClubTabShowActiveCheckboxHandler(sender as CheckBox);
        }
        private void GoldClubTabShowActiveCheckboxHandler(CheckBox checkBox)
        {
            bool isChecked = (bool)checkBox.IsChecked;
            if (isChecked)
            {
                //Make relevant input fields visible and vice versa
            }
            else
            {
                //Make relevant input fields visible and vice versa
            }
        }

        private void GoldClubTabShowActiveCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            GoldClubTabShowInactiveCheckboxHandler(sender as CheckBox);
        }

        private void GoldClubTabShowInactiveCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            GoldClubTabShowInactiveCheckboxHandler(sender as CheckBox);
        }

        private void GoldClubTabShowInactiveCheckboxHandler(CheckBox checkBox)
        {
            bool isChecked = (bool)checkBox.IsChecked;
            if (isChecked)
            {
                //Make relevant input fields visible and vice versa
            }
            else
            {
                //Make relevant input fields visible and vice versa
            }
        }

        #endregion GoldClubTab

        #region Utilities
        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;
            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);
                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }
            return foundChild;
        }

        /// <summary>
        /// Finds the visual child.
        /// </summary>
        /// <typeparam name="childItem">The type of the child item.</typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    yield return (T)child;
                }
                else
                {
                    var childOfChild = FindVisualChildren<T>(child);
                    if (childOfChild != null)
                    {
                        foreach (var subchild in childOfChild)
                        {
                            yield return subchild;
                        }
                    }
                }
            }
        }












        #endregion

        private void Year_Loaded(object sender, RoutedEventArgs e)
        {
            Year.Text = "Put Year Here!";
        }

        private void BookingTabCustomerNameTextBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            BookingTabCustomerNameTextBox1.Text = "";
        }

        private void BookingTabEmailTextBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            BookingTabEmailTextBox1.Text = "";
        }
    }
}
