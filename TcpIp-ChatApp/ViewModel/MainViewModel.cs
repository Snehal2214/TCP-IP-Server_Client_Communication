using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TcpIp_ChatApp.Command;
using TcpIp_ChatApp.Model;
using TcpIp_ChatApp.Net;
using ClosedXML.Excel;
using System.ComponentModel;
using System.Messaging;
using TcpIp_ChatApp.Net.IO;




namespace TcpIp_ChatApp.ViewModel
{
    public class MainViewModel 
    {
        
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }


        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand SendExcelDataCommand { get; set; }


        private Server _server;
        public string Username { get; set; }
        public string Message { get; set; }

        
        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();


            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.userDisconnectEvent += RemoveUser;

            _server.acknowledgmentReceivedEvent += AcknowledgmentReceived;

            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), o=> !string.IsNullOrEmpty(Username));
            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));

            string excelFilePath = @"C:\Users\Sneha\Desktop\DemoData.xlsx";
            SendExcelDataCommand = new RelayCommand(async o => await _server.SendExcelDataToServer(excelFilePath), o => true);
        
        }

        private void RemoveUser()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageReceived()
        {
            var msg = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage(),
            };
            if (!Users.Any(x => x.UID == user.UID)) 
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
        private void AcknowledgmentReceived(string acknowledgment)
        {
            
            Application.Current.Dispatcher.Invoke(() => Messages.Add($"Server: {acknowledgment}"));
            
        }


    }
}