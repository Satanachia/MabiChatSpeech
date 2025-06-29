﻿using MabiChatSpeech;
using SharpPcap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Linq;
using static MabiChatSpeech.MabiChat;
using static MabiChatSpeech.Program;

namespace MabiChatSpeech
{
    public enum ClinetStatus { OFF, ON, CHARASEL, ONLINE }
    public enum CharacterTypes : byte { User = 0x00, Pet = 0x01, Doll = 0x03 ,Npc = 0xf0 }
    public enum PacketModes { Chat, Dump,Analysys }
    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_TCPROW_OWNER_PID
    {
        public int State;
        public int LocalAddr;
        public int LocalPort;
        public int RemoteAddr;
        public int RemotePort;
        public int OwningPid;
    }

    enum TCP_TABLE_CLASS
    {
        TCP_TABLE_BASIC_LISTENER,
        TCP_TABLE_BASIC_CONNECTIONS,
        TCP_TABLE_BASIC_ALL,
        TCP_TABLE_OWNER_PID_LISTENER,
        TCP_TABLE_OWNER_PID_CONNECTIONS,
        TCP_TABLE_OWNER_PID_ALL,
        TCP_TABLE_OWNER_MODULE_LISTENER,
        TCP_TABLE_OWNER_MODULE_CONNECTIONS,
        TCP_TABLE_OWNER_MODULE_ALL
    };
    public struct st_MabiPort
    {
        public bool uflag;
        public int State;
        public string LocalAddr;
        public int LocalPort;
        public string RemoteAddr;
        public int RemotePort;

    }
    public struct st_MabiServer
    {
        public string name;
        public string ip;
    }

    public struct st_MabiServerList
    {
        public string ip;
        public int svno;
        public int chno;
        public st_MabiServerList(string p1 , int p2 , int p3)
        {
            this.ip = p1;
            this.svno = p2;
            this.chno = p3;
        }
    };

    // Chat Data
    public struct ChatData
    {
        public string CharacterName;
        public CharacterTypes CharacterType;
        public string ChatWord;
    }

    //Nic
    public struct st_adapter
    {
        public string Description;
        public string Name;
        public string ipv4Addr;
        public NetworkInterfaceType ifacetype;
    }

    // Event Interface
    public interface IMabiPacketObject
    {
        event EventHandler ConnectEvent;
        event EventHandler ChatEvent;
        event EventHandler PacketEvent;
    }
    public class MabiPacketEventArgs : EventArgs
    {
        public string CharacterName;
        public CharacterTypes CharacterType;
        public string ChatWord;
        public string PacketDump;
        public ClinetStatus csts;
        public bool cap_sts;
        public string svname;
        public string svip;
    }

    // Mabinogi Packet Class
    public class MabiPacket
    {
        [DllImport("iphlpapi.dll")]
        extern static int GetExtendedTcpTable(IntPtr pTcpTable, ref int pdwSize,
        bool bOrder, uint ulAf, TCP_TABLE_CLASS TableClass, int Reserved);

        public static List<ChatData> chatDatas = new List<ChatData>();

        private System.Timers.Timer WDT = new System.Timers.Timer();
        public ClinetStatus csts;
        public int PortNo = 0;
        private int pid;
        public List<st_MabiPort> PortList = new List<st_MabiPort>();
        private string svip = "";
        public string svname = "";
        public bool cap_sts = false;
        public PacketModes PacketMode = PacketModes.Chat;
        private string localip = "";
        private int localPort;
        private static CaptureDeviceList NetDevs = CaptureDeviceList.Instance;
        private static ILiveDevice capdev;
        private static byte[] tcpbuff = new byte[1024 * 1024 * 4];
        private static int tcpblen;
        private static int bpos = 0;
        private static int pushcnt = 0;
        private static string dumppath = "";

        static public st_MabiServerList[] MabiServerList =
        {
        new st_MabiServerList("52.196.142.146",1,1),
        new st_MabiServerList("54.199.99.252",1,2),
        new st_MabiServerList("52.197.188.102",1,3),
        new st_MabiServerList("52.194.60.149",1,4),
        new st_MabiServerList("13.112.198.165",1,5),
        new st_MabiServerList("52.192.125.124",1,6),
        new st_MabiServerList("52.194.32.74",1,7),
        new st_MabiServerList("52.199.154.240",2,1),
        new st_MabiServerList("13.115.244.90",2,2),
        new st_MabiServerList("18.182.71.159",2,3),
        new st_MabiServerList("52.192.156.33",2,4),
        new st_MabiServerList("52.196.130.66",2,5),
        new st_MabiServerList("52.68.142.88",2,6),
        new st_MabiServerList("52.193.116.167",2,7),
        new st_MabiServerList("52.196.108.60",3,1),
        new st_MabiServerList("52.194.180.89",3,2),
        new st_MabiServerList("13.115.108.145",3,3),
        new st_MabiServerList("13.114.246.104",3,4),
        new st_MabiServerList("54.250.200.139",3,5),
        new st_MabiServerList("52.69.23.101",3,6),
        new st_MabiServerList("52.193.39.60",3,7)
        };
        
        private static string GetNicName(string ip)
        {
            string nicname = "";

            // 物理インターフェース情報をすべて取得
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            // 各インターフェースごとの情報を調べる
            foreach (var adapter in interfaces)
            {
                // 有効なインターフェースのみを対象とする
                if (adapter.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                // インターフェースに設定されたIPアドレス情報を取得
                var properties = adapter.GetIPProperties();

                // 設定されているすべてのユニキャストアドレスについて
                foreach (var unicast in properties.UnicastAddresses)
                {
                    if (unicast.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if ($"{unicast.Address}" == ip)
                        {
                            nicname = $"{adapter.Id}";
                        }
                    }
                    else if (unicast.Address.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        // IPv6アドレス
                    }
                }
            }
            return nicname;
        }

        // タイマー監視
        private void onWDT(object sender, ElapsedEventArgs e)
        {
            ClinetStatus sts = ClinetStatus.OFF;
            string ip = "";
            string name = "";

            this.pid = GetMabinogiPid();
            if (this.pid != 0)
            {
                sts = ClinetStatus.ON;
                PortList.Clear();
                PortList = ScanMabiPort(this.pid);
                if (PortList.Count > 0)
                {
                    sts = ClinetStatus.CHARASEL;
                    if (localip == "")
                    {
                        // ローカルIP確定 キャプチャデバイスを指定する
                        localip = PortList[0].LocalAddr;
                        var nic = GetNicName(localip);
                        capdev = null;
                        foreach (var dev in NetDevs)
                        {
                            if (dev.Name.Contains(nic))
                            {
                                capdev = dev;
                            }
                        }
                    }
                    // Caputure開始
                    if ((capdev != null) && (capdev.Started == false))
                    {
                        capdev_start();
                    }

                    var sv = PortList.Find(x => x.RemotePort == 11020);
                    if (sv.RemotePort == 11020)
                    {
                        // Localポートをホールド
                        PortNo = sv.LocalPort;
                        // サーバーリストからマッチを探す
                        var csv = ServerList.Find(x => x.ip == sv.RemoteAddr);
                        if (csv.name.Length > 0)
                        {
                            sts = ClinetStatus.ONLINE;
                            ip = csv.ip;
                            name = csv.name;
                            // ここにPort
                            localPort = sv.LocalPort;
                        }
                    }
                    else
                    {
                        sts = ClinetStatus.CHARASEL;
                    }
                }
            }
            if ((sts != csts) || (svip != ip))
            {
                svip = ip;
                svname = name;
                csts = sts;
                Connect();
            }
        }

        public event EventHandler ConnectEvent;
        void Connect()
        {
            var e = new MabiPacketEventArgs();
            e.csts = csts;
            if (capdev != null)
            {
                e.cap_sts = capdev.Started;
            }
            else
            {
                e.cap_sts = false;
            }
            e.svname = svname;
            e.svip = svip;

            OnConnect(e);
        }
        protected virtual void OnConnect(MabiPacketEventArgs e)
        {
            ConnectEvent?.Invoke(this, e);
        }
        public event EventHandler ChatEvent;
        void Chat(ChatData d)
        {
            var e = new MabiPacketEventArgs();
            e.CharacterName = d.CharacterName;
            e.ChatWord = d.ChatWord;
            e.CharacterType = d.CharacterType;
            OnChat(e);
        }
        protected virtual void OnChat(MabiPacketEventArgs e)
        {
            ChatEvent?.Invoke(this, e);
        }
        public event EventHandler PacketEvent;
        void PacketDumps(string s)
        {
            var e = new MabiPacketEventArgs();
            e.PacketDump = s;
            OnPacketDump(e);
        }
        protected virtual void OnPacketDump(MabiPacketEventArgs e)
        {
            PacketEvent?.Invoke(this, e);
        }

        public MabiPacket()
        {
            var ts = DateTime.Now;
            LoadChanelList();
            dumppath = $"Dump_{ts:yyyyMMdd_HHmmss}";
            csts = ClinetStatus.OFF;
            WDT.Interval = 1000;
            WDT.Elapsed += onWDT;
            WDT.Start();
        }
        private static string ipstr(int addr)
        {
            var b = BitConverter.GetBytes(addr);
            return string.Format("{0}.{1}.{2}.{3}", b[0], b[1], b[2], b[3]);
        }

        private static int htons(int i)
        {
            var tmp = (((0x000000ff & i) << 8) + ((0x0000ff00 & i) >> 8))
                       + ((0x00ff0000 & i) << 8) + ((0xff000000 & i) >> 8);
            return (int)tmp;
        }

        private static int GetMabinogiPid()
        {
            System.Diagnostics.Process[] ps =
                System.Diagnostics.Process.GetProcessesByName("client");
            foreach (System.Diagnostics.Process p in ps)
            {
                return (p.Id);
            }
            return (0);
        }
        public static List<st_MabiServer> ServerList = new List<st_MabiServer>();

        public List<st_MabiServer> SVList()
        {
            return (ServerList);
        }
        private static List<st_MabiPort> ScanMabiPort(int pid)
        {
            var retval = new List<st_MabiPort>();

            int size = 0;
            uint AF_INET = 2; // IPv4
            //必要サイズの取得            
            GetExtendedTcpTable(IntPtr.Zero, ref size, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
            var p = Marshal.AllocHGlobal(size);//メモリ割当て
            //TCPテーブルの取得            
            if (GetExtendedTcpTable(p, ref size, true, AF_INET, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0) == 0)
            {
                var num = Marshal.ReadInt32(p);//MIB_TCPTABLE_OWNER_PID.dwNumEntries(データ数)
                var ptr = IntPtr.Add(p, 4);
                for (int i = 0; i < num; i++)
                {
                    var o = (MIB_TCPROW_OWNER_PID)Marshal.PtrToStructure(ptr, typeof(MIB_TCPROW_OWNER_PID));

                    if (o.RemoteAddr == 0)
                    {
                        o.RemotePort = 0;//RemoteAddrが0の場合は、RemotePortも0にする
                    }

                    if (o.OwningPid == pid)
                    {
                        // マビノギが開いているポート 
                        st_MabiPort arec = new st_MabiPort();
                        arec.uflag = true;
                        arec.State = o.State;
                        arec.LocalPort = htons(o.LocalPort);
                        arec.LocalAddr = ipstr(o.LocalAddr);
                        arec.RemotePort = htons(o.RemotePort);
                        arec.RemoteAddr = ipstr(o.RemoteAddr);

                        retval.Add(arec);
                    }
                    ptr = IntPtr.Add(ptr, Marshal.SizeOf(typeof(MIB_TCPROW_OWNER_PID)));//次のデータ
                }
                Marshal.FreeHGlobal(p);  //メモリ開放
            }
            return (retval);
        }
        private static void LoadChanelList()
        {
            ServerList.Clear();
            string path = @"MabiServerList.txt";
            var lines = File.ReadAllLines(path, Encoding.GetEncoding("utf-8"));
            foreach (var line in lines)
            {
                string[] coldata = line.Split(',');
                if (coldata.Length > 1)
                {
                    var item = new st_MabiServer();
                    item.name = coldata[0];
                    item.ip = coldata[1];
                    ServerList.Add(item);
                }
            }
        }
        private static List <string> tcp_blist = new List<string>();
        private static int push_packet(PacketDotNet.TcpPacket p, int pos, byte[] pay, int len)
        {
            // Debug.Print($"Push P :{pushcnt}/{pos}/{len}");
            Array.Copy(pay, 0, tcpbuff, pos, len);
            tcpblen = pos + len;
            tcp_blist.Add($"  Flags 0x{p.Flags:x2},Seq {p.SequenceNumber:d10},Ack {p.AcknowledgmentNumber:d10},len {p.PayloadData.Length:d10}");
            return ( tcpblen );
        }


        private static string packet_string(byte[] buff , int pos )
        {
            string ret_val = "";
            Int16 len = System.BitConverter.ToInt16(buff, pos);
            byte[] ret_byte = new byte[len];
            Array.Copy(buff, pos + 2, ret_byte, 0, len);
            var encoding = Encoding.GetEncoding("UTF-8");
            ret_val = encoding.GetString(ret_byte);
            return (ret_val);
        }
        struct PacketChatData
        {
            public byte CNameType;
            public string CName;
            public byte CWordType;
            public string CWord;
        }
        private static PacketChatData packet_getChat(byte [] buff,int pos)
        {

            /*
             * [06 00](文字数)[1Byte](文字データ)[nByte][06 00](文字数)[1Byte](文字データ)
             * 
             * 
             * 
             */

            PacketChatData ret = new PacketChatData();
            int ofst = pos;
            ret.CNameType =  buff[ofst];
            ofst += 2;
            var CNBlen = buff[ofst];
            ofst += 1;
            byte[] cn = new byte[CNBlen];
            Array.Copy(buff, ofst, cn, 0, CNBlen);
            ofst += CNBlen;
            var encoding = Encoding.GetEncoding("UTF-8");
            ret.CName = encoding.GetString(cn);
            ret.CName = ret.CName.Replace("\0", "");

            ret.CWordType = buff[ofst];
            ofst += 2;
            var CWBlen = buff[ofst];
            ofst += 1;
            byte[] cw = new byte[CWBlen];
            Array.Copy(buff, ofst, cw, 0, CWBlen );
            ret.CWord = encoding.GetString(cw);
            ret.CWord = ret.CWord.Replace("\0", ""); 

            return (ret);

        }
        
        private enum PacketType { Chat, Echa, Other ,Error}
        private static PacketType IsPacketType( int blocktop , int blocklen )
        {
            int pos = 5 + blocktop;
            if ((tcpbuff[pos+0] == 0x03) &&
                 (tcpbuff[pos+1] == 0x00) &&
                 (tcpbuff[pos+2] == 0x00) &&
                 (tcpbuff[pos+3] == 0x52) &&
                 (tcpbuff[pos+4] == 0x6c) &&
                 (tcpbuff[pos+5] == 0x00) &&
                 (tcpbuff[pos+6] == 0x10))
            {
                return (PacketType.Chat);
            }
            else if ((tcpbuff[pos+0] == 0x03) &&
                         (tcpbuff[pos+1] == 0x00) &&
                         (tcpbuff[pos+2] == 0x00) &&
                         (tcpbuff[pos+3] == 0x52) &&
                         (tcpbuff[pos+4] == 0x7c) &&
                         (tcpbuff[pos+5] == 0x00) &&
                         (tcpbuff[pos+6] == 0x10) &&
                         (tcpbuff[pos+7] == 0x00) &&
                         (tcpbuff[pos+8] == 0x00) &&
                         (tcpbuff[pos+9] == 0x00))
            {
                return (PacketType.Echa);
            }
            else
            {
                return (PacketType.Other);
            }
        }
        //参照：http://nanoappli.com/blog/archives/2259
        //---------------------------------------------------------------------------
        /// <summary>
        /// 指定されたURLの画像をImage型オブジェクトとして取得する
        /// </summary>
        /// <param name="url">画像データのURL(ex: http://example.com/foo.jpg) </param>
        /// <returns>         画像データ</returns>
        //---------------------------------------------------------------------------
        public static System.Drawing.Image loadImageFromURL(string url)
        {
            int buffSize = 65536; // 一度に読み込むサイズ
            MemoryStream imgStream = new MemoryStream();

            //------------------------
            // パラメータチェック
            //------------------------
            if (url == null || url.Trim().Length <= 0)
            {
                return null;
            }

            //----------------------------
            // Webサーバに要求を投げる
            //----------------------------
            WebRequest req = WebRequest.Create(url);
            BinaryReader reader = new BinaryReader(req.GetResponse().GetResponseStream());

            //--------------------------------------------------------
            // Webサーバからの応答データを取得し、imgStreamに保存する
            //--------------------------------------------------------
            while (true)
            {
                byte[] buff = new byte[buffSize];

                // 応答データの取得
                int readBytes = reader.Read(buff, 0, buffSize);
                if (readBytes <= 0)
                {
                    // 最後まで取得した->ループを抜ける
                    break;
                }

                // バッファに追加
                imgStream.Write(buff, 0, readBytes);
            }

            return new Bitmap(imgStream);
        }
        //
        // パケット解析
        //
        private static List <ChatData> analyses_packet2(int len) // len は tcpblen
        {
            var ret = new List<ChatData>();
            string [] expression_ = {
                "(愛)", "(悪)", "(安)", "(怪)", "(感)", "(喜)",
                "(奇)", "(嬉)", "(輝)", "(疑)", "(泣)", "(叫)",
                "(恐)", "(驚)", "(緊)", "(苦)", "(嫌)", "(幸)",
                "(混)", "(惨)", "(酸)", "(邪)", "(照)", "(笑)",
                "(衝)", "(情)", "(真)", "(辛)", "(酔)", "(正)",
                "(静)", "(暖)", "(恥)", "(痛)", "(怒)", "(悲)",
                "(疲)", "(普)", "(変)", "(呆)", "(萌)", "(黙)",
                "(優)", "(痒)", "(睨)",

            };


            // Block単位でサーチ
            try
            {
                for (int b = 0; b < tcpblen;)
                {
                    byte id = tcpbuff[b];
                    Int32 blocklen = System.BitConverter.ToInt32(tcpbuff, b + 1);
                    if (blocklen <= 0)
                    {
                        break;
                    }
                    var btype = IsPacketType(b, blocklen);
                    int bf = b + 5;
                    int bd = 0;

                    // Block判別 オープンチャット判別
                    if (btype==PacketType.Chat)
                    {

                        var val = new ChatData();
                        val.CharacterType = (CharacterTypes)tcpbuff[bf + 7];


                        // 吹き出し
                        if (tcpbuff[bf + 7] == 0x00)
                        {
                            // ユーザーの吹き出しデータ開始位置
                            bd = bf + 18;
                        }
                        else
                        {
                            // NPCの吹き出データ開始位置
                            bd = bf + 18;
                        }
                        // 03 00 01 00 のパターン位置
                        for (int i = bf+8 ; i< blocklen; i++)
                        {
                            if (tcpbuff[i] == 0x03)
                            {
                                if (tcpbuff[i + 1] == 0x00)
                                {
                                    if (tcpbuff[i + 2] == 0x01)
                                    {
                                        if (tcpbuff[i + 3] == 0x00)
                                        {
                                            bd = i + 4;
                                        }
                                    }
                                }
                            }
                        }

                        var cd = packet_getChat(tcpbuff, bd);

                        // 顔エモの削除
                        foreach (var ex in expression_ )
                        {
                            cd.CWord = cd.CWord.Replace(ex, "");
                        }

                        if ( cd.CWord.Length > 0 )
                        {
                            val.CharacterName = cd.CName;
                            val.ChatWord = cd.CWord;
                            ret.Add(val);
                        }

                    }
                    // 絵チャ判別
                    else if ( ( btype == PacketType.Echa ) && ( Program.__Echa != 0 ))
                    {
                        var val = new ChatData();
                        val.CharacterType = (CharacterTypes)tcpbuff[bf + 7];


                        // 05 00 のパターン位置
                        for (int i = bf + 14; i < blocklen; i++)
                        {
                            if (tcpbuff[i] == 0x05)
                            {
                                if (tcpbuff[i + 1] == 0x00)
                                {
                                            bd = i + 2;
                                }
                            }
                        }

                        var cd = packet_getChat(tcpbuff, bd);

                        if (cd.CWord.Length > 0)
                        {
                            var img = loadImageFromURL(cd.CWord);
                            string[] urlname = cd.CWord.Split('/');

                            if (Program.__Echa > 1) //保存する
                            {
                                img.Save(__SavePath + "\\echa\\" + urlname[urlname.Length - 1], ImageFormat.Png);
                            }
                            val.CharacterName = cd.CName;
                            val.ChatWord = urlname[urlname.Length - 1];
                            if ( ( Program.__Echa == 1 ) || ( Program.__Echa == 2 ) )
                            {
                                ret.Add(val);
                            }
                        }
                    }
                    b += blocklen;
                }
            }
            catch
            {
            }
            Debug.Print($" {ret.Count:d2} ");
            return ret;
        }
        public static void chatdatas_add(ChatData data)
        {
            ChatData item = chatDatas.Find(x => x.CharacterName == data.CharacterName && x.CharacterType == data.CharacterType);
            if (item.CharacterName == null)
            {
                chatDatas.Add(data);
            }
        }
        private void device_OnPacketArrival(object sender, PacketCapture e)
        {
            try
            {
            var time = e.Header.Timeval.Date;
            var rawPacket = e.GetPacket();
            var packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
            var tcpPacket = packet.Extract<PacketDotNet.TcpPacket>();
            if (tcpPacket != null)
            {
                var ipPacket = (PacketDotNet.IPPacket)tcpPacket.ParentPacket;
                System.Net.IPAddress srcIp = ipPacket.SourceAddress;
                System.Net.IPAddress dstIp = ipPacket.DestinationAddress;
                int srcPort = tcpPacket.SourcePort;
                int dstPort = tcpPacket.DestinationPort;


　              // local側 ipアドレス＋ポート番号で　パケットデータの仕分け
                var lip = $"{dstIp}:{dstPort}";
                var sip = $"{srcIp}";
                // 多重起動しているClient(VM上)のメッセージを除外
                if ( PortNo != dstPort )
                {
                    return;
                }

                if (svip != sip)
                {
                    // サーバーリストからマッチを探す
                    var csv = ServerList.Find(x => x.ip == sip);
                    if (csv.name.Length > 0)
                    {
                        svip = csv.ip;
                        svname = csv.name;
                    }
                    Connect();
                }

                if (PacketMode == PacketModes.Dump)
                {
                    string dumpstr = dumptext(tcpPacket);
                    PacketDumps(dumpstr);
//                    PacketDumpWrite(dumpstr);
                }

                if (tcpPacket.Push == false)
                {
                    // パケットの続きあり
                    bpos = push_packet(tcpPacket , bpos, tcpPacket.PayloadData, tcpPacket.PayloadData.Length);
                    if (tcpPacket.PayloadData.Length > 0)
                    {
                        pushcnt++;
                    }
                    return;
                }
                bpos = push_packet(tcpPacket , bpos, tcpPacket.PayloadData, tcpPacket.PayloadData.Length);

                if (PacketMode == PacketModes.Chat)
                {
                    var chats = analyses_packet2(tcpblen);
                    foreach (var chat in chats )
                    {
                        if ((chat.ChatWord != "") && (chat.CharacterName != ""))
                        {
                            chatdatas_add(chat);
                            //チャット受信でイベント
                            Chat(chat);
                        }
                    }
                }
                else if (PacketMode == PacketModes.Dump)
                {
                    string dumpstr = Analysys_packet();
                    PacketDumps(dumpstr);
                }
                else if (PacketMode == PacketModes.Analysys)
                {
                    string dumpstr  = Analysys_packet();
                    PacketDumps(dumpstr);
                }

                bpos = 0;
                pushcnt = 0;
                tcp_blist.Clear();
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message , "OnPacketArrive Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void device_OnCaptureStopped(object sender, CaptureStoppedEventStatus status)
        {
            svip = "";
            svname = "";
            cap_sts = capdev.Started;
        }

        private void capdev_start()
        {
            capdev.Open(DeviceModes.Promiscuous, 1000);
            capdev.OnPacketArrival += device_OnPacketArrival;
            capdev.OnCaptureStopped += device_OnCaptureStopped;
            capdev.Filter = $"tcp src port 11020 and dst host {localip}";// and dst port {lp}";
            capdev.StartCapture();
            cap_sts = capdev.Started;

            Connect();

        }
        private void capdev_stop()
        {
            svip = "";
            svname = "";
            cap_sts = capdev.Started; 
        }

        static private string dumptext_Format(byte[] payload , int len )
        {
            string lstr = "";

            if (len < 1)
            {
                return ("");
            }

            lstr += "     | +0 +1 +2 +3 +4 +5 +6 +7 +8 +9 +A +B +C +D +E +F | 0123456789ABCDEF |" + Environment.NewLine;
            lstr += "-----+-------------------------------------------------+------------------|" + Environment.NewLine;

            for (int i = 0; i < len; i += 16)
            {
                lstr += $"{i:X4} | ";
                for (int j = 0; j < 16; j++)
                {
                    if (i + j >= len)
                    {
                        var feed = 16 - j;
                        for (int f = feed; f > 0; f--)
                        {
                            lstr += "   ";
                        }
                        break;
                    }
                    lstr += $"{payload[i + j]:x02} ";
                }

                lstr += "| ";
                for (int j = 0; j < 16; j++)
                {
                    if (i + j >= len)
                    {
                        var feed = 16 - j;
                        for (int f = feed; f > 0; f--)
                        {
                            lstr += " ";
                        }
                        lstr += " ";
                        break;
                    }

                    try
                    {
                        var Moji = "";
                        if ((payload[i + j] & 0B1110_0000) == (0B1110_0000))
                        { // 3
                            if (i + j + 2 >= len)
                            { // Over Range // 1ByteData

                            }
                            else
                            {
                                /* UTF8コード*/
                                byte[] uni = new byte[3];
                                uni[0] = payload[i + j];
                                uni[1] = payload[i + j + 1];
                                uni[2] = payload[i + j + 2];

                                if (((0xe0 <= uni[0]) && (uni[0] <= 0xef)) &&
                                     ((0x80 <= uni[1]) && (uni[1] <= 0xBF)) &&
                                     ((0x80 <= uni[2]) && (uni[2] <= 0xBF)))
                                {
                                    var encoding = Encoding.GetEncoding("UTF-8");
                                    Moji = encoding.GetString(uni);

                                    if (j < 13)
                                    {
                                        Moji += "_";
                                    }
                                    else if (j == 13)
                                    {

                                        Moji += "_ ";
                                    }
                                    else if (j == 14)
                                    {
                                        Moji += "=";
                                    }

                                    j += 2;
                                }
                            }
                        }

                        if (Moji.Length == 0)
                        {
                            if ((0x20 <= payload[i + j]) && (payload[i + j] <= 0x7e))
                            {
                                char c = (char)payload[i + j];
                                Moji += c;
                            }
                            else
                            {
                                Moji += ".";
                            }

                            if (j == 15)
                            {
                                Moji += " ";
                            }
                        }

                        lstr += Moji;
                    }
                    catch
                    {
                        lstr += " ";
                    }
                }

                lstr += "|";

                lstr += Environment.NewLine;
            }
            lstr += Environment.NewLine;

            return (lstr);

        }
        private static void pop_packet()
        {
            if (pushcnt == 0)
            {
                return;
            }

            var tm = DateTime.Now;
            var fn = __SavePath + "\\" + dumppath ;
            int AD = 0;
            if (!System.IO.Directory.Exists(fn))
            {
                Directory.CreateDirectory(fn);
            }
            fn += "\\dump_" + $"{tm:HHmmssff}";

            List<string> block = new List<string>();
            bool wf = false;

            try
            {

                for (Int32 idx = 0; idx < tcpblen;)
                {
                    Int32 blocksize = System.BitConverter.ToInt32(tcpbuff, idx + 1);
                    Int32 NextAddr = idx + blocksize;
                    string bs = "";
                    for ( int i = 0 ; i < 8; i++ )
                    {
                        bs += $"{tcpbuff[idx + 5 + i]:x2} ";
                    }
                    block.Add($"ID 0x{tcpbuff[idx]:x2},BlockSize 0x{blocksize:x8},NextBlock {NextAddr:x8},Data "+bs);
                    if ( ( idx + blocksize) >= tcpblen )
                    {
                        if ( idx == 0)
                        {
                            wf = false;
                        }
                        else
                        {
                            wf = true;
                        }
                    }
                    if (blocksize <= 0)
                    {
                        break;
                    }
                    idx += blocksize;
                    AD = idx;
                }
            }
            catch
            {
                wf = true;
                fn += "_Error";
            }

            if (AD != tcpblen )
            {
                wf = true;
                fn += "_ErrorBlock";
            }

            if ( wf == true )
            {
                using (FileStream fs = new FileStream(fn + ".bin", FileMode.Create, FileAccess.ReadWrite))
                {
                    fs.Write(tcpbuff, 0, tcpblen);
                }

                using (StreamWriter sw = new StreamWriter(fn + ".txt", false, Encoding.UTF8))
                {
                    sw.WriteLine($"{fn} Length:{tcpblen} 0x{tcpblen:x} Count:{pushcnt}");
                    foreach (var he in tcp_blist)
                    {
                        sw.WriteLine(he);
                    }

                    foreach (string s in block)
                    { 
                        sw.WriteLine(s);
                    }
                }
            }
        }

        private static string Analysys_packet()
        {
            string ret_val = "";
            var tm = DateTime.Now;
            int bcount = 1;
            int Ad = 0;
            if( tcp_blist.Count > 1 )
            {
                ret_val += "SegmentData" + Environment.NewLine;
            }
            else
            {
                ret_val += "SingleData" + Environment.NewLine;
            }

            foreach (var he in tcp_blist)
            {
                ret_val+= he;
                ret_val += Environment.NewLine;
            }
            tcp_blist.Clear ();
            ret_val += $"  Segmant Length:{tcpblen} 0x{tcpblen:x8} Count:{pushcnt}"+ Environment.NewLine ;

            if (tcpblen < 8 )
            {
                for (int i = 0; i < tcpblen; i++)
                {
                    ret_val += $"{tcpbuff[i]:x2} ";
                }
                ret_val += Environment.NewLine;
                return (ret_val);
            }

            // データブロック単位
            try
            {
                for (Int32 idx = 0; idx < tcpblen;)
                {
                    Int32 blocksize = System.BitConverter.ToInt32(tcpbuff, idx + 1);
                    Int32 NextAddr = idx + blocksize;
                    string bs = "";
                    for (int i = 0; i < 8; i++)
                    {
                        if (idx + 5 + i >= tcpblen)
                        {
                            break;
                        }
                        bs += $"{tcpbuff[idx + 5 + i]:x2} ";
                    }
                    ret_val += ($"  {bcount:d4},ID 0x{tcpbuff[idx]:x2},BSize 0x{blocksize:x8},BNext {NextAddr:x8},Data " + bs + Environment.NewLine);
                    bcount++ ;
                    if (blocksize <= 0)
                    {
                        break;
                    }
                    idx += blocksize;
                    Ad = idx;
                }
            }
            catch
            {
                ret_val += Environment.NewLine + $"_Error:{bcount}"+ Environment.NewLine;
            }
            if ( Ad != tcpblen )
            {
                ret_val += Environment.NewLine + $"_Block Error: 0x{Ad:x8}" + Environment.NewLine ;
            }
            ret_val += Environment.NewLine;
            return (ret_val);
        }


        private string dumptext( PacketDotNet.TcpPacket p )
        {
            var tm = DateTime.Now;

            if (p.PayloadData == null)
            {
                return ("");
            }
            var len = p.PayloadData.Length;
            var payload = p.PayloadData;
            string lstr = "";
            lstr += $"Time:{tm} (Len):{len} (WSize):{p.WindowSize:d8}" + Environment.NewLine;
            lstr += $"(PSH){p.Push} (Flags)0x{p.Flags:X} (S){p.SequenceNumber} / (A){p.AcknowledgmentNumber}" + Environment.NewLine; ;
            lstr += dumptext_Format(payload, len);
            return (lstr);

        }
        private void PacketDumpWrite(string s)
        {
            var fn = __SavePath + "\\" + dumppath;
            if (!System.IO.Directory.Exists(fn))
            {
                Directory.CreateDirectory(fn);
            }
            fn += "\\Dump.txt";
            File.AppendAllText(fn, s);
        }

    }
}
