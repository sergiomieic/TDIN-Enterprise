﻿using Common;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Book_Enterprise;

namespace Store
{
    public partial class StoreGUI : Form
    {
        protected StoreCommunicationHandler commHandler;
        private ArrayList booksList { get; set; }

        public ArrayList acceptOrdersList { get; set; }

        public StoreGUI(StoreCommunicationHandler handler)
        {
            commHandler = handler;
            commHandler.gui = this;
            InitializeComponent();
        }

        private void StoreGUI_Load(object sender, EventArgs e)
        {
            commHandler.sendMsg("getOrders", "");
        }

        private void sellButton_Click(object sender, EventArgs e)
        {
            ListViewItem book;

            try
            {
                book = listView1.SelectedItems[0];
            }
            catch (Exception exception)
            {
                return;
            }

            SellOrder sell = new SellOrder(book);

            sell.commHandler = commHandler;

            sell.Show();
        }

        private void orderButton_Click(object sender, EventArgs e)
        {
            ListViewItem book;

            try
            {
                book = listView1.SelectedItems[0];
            }
            catch (Exception exception)
            {
                return;
            }

            OrderCliForm form = new OrderCliForm(book.SubItems[0].Text);
            form.commHandler = commHandler;
            form.Show();
        }

        private void acceptButton_Click(object sender, EventArgs e)
        {            
            ListViewItem order;

            try
            {
                order = ordersListView.SelectedItems[0];
            }
            catch (Exception exception)
            {
                return;
            }

            // TODO (check tasks done below)
            //update stock [ ]
            //update order [X]
            //send email   [ ]

            //  order.SubItems
            //updt book stock GUI  there is a method ehre for this
            
            ListViewItem book = listView1.FindItemWithText(order.Name);
            
            ShipOrderMessage msg = new ShipOrderMessage()
            {
                bookTitle = order.SubItems[0].Text,
                qtd = int.Parse(order.SubItems[1].Text),
                id = order.SubItems[2].Text
            };

            commHandler.sendMsg("checkStockOrders", msg.getJSON());

            ordersListView.BeginInvoke((Action)(() =>
            {
                ordersListView.SelectedItems[0].Remove();
            }));
        }

        public void updateBookStock(JObject json)
        {
            listView1.BeginInvoke((Action)(() =>
                {
                    listView1.FindItemWithText(json["bookTitle"].ToString()).SubItems[1].Text = json["stock"].ToString();
                }));
        }


        public void showInitialBooks(JObject books)
        {
            var booksArray = (JArray) books["data"];

            booksList = new ArrayList(JsonConvert.DeserializeObject<List<Book>>(booksArray.ToString()));

            ordersListView.BeginInvoke((Action)(() =>
            {
                foreach (Book book in booksList)
                {
                    ListViewItem lvItem = new ListViewItem(book.Title);
                    lvItem.SubItems.Add(book.Stock.ToString());
                    lvItem.SubItems.Add(book.Price.ToString());

                    listView1.Items.Add(lvItem);
                }
            }));
        }

        
        public void initialOrdersView(JObject data)
        {
            var toAcceptOrders = (JArray)data["data"];

            acceptOrdersList = new ArrayList(JsonConvert.DeserializeObject<List<Order>>(toAcceptOrders.ToString()));                     

            ordersListView.BeginInvoke((Action)(() =>
            {
                foreach (Order order in acceptOrdersList)
                {
                    ListViewItem lvItem = new ListViewItem(order.BookTitle);
                    lvItem.SubItems.Add(order.Quantity.ToString());
                    lvItem.SubItems.Add(order.OrderId.ToString());

                    ordersListView.Items.Add(lvItem);
                }
            }));
        }

        public void addOrderView(string data)
        {
            Console.WriteLine(">>>>>>>>", data);

            JObject shipOrder = JObject.Parse(data);


            ordersListView.BeginInvoke((Action)(() =>
            {
                ListViewItem lvItem = new ListViewItem(shipOrder["bookTitle"].ToString());
                lvItem.SubItems.Add(shipOrder["qtd"].ToString());
                lvItem.SubItems.Add(shipOrder["id"].ToString());

                ordersListView.Items.Add(lvItem);
            }));
        }

      
    }
}
