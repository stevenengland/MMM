#property copyright "Copyright 2019, StEn"
#property link      "https://steven-england.info"
#property version   "2.0"

#import "StEn.MMM.Mql.Telegram.dll"
// Available functions:
	// string Initialize(string apiKey, int timeout);
	// string SetDebugOutput(bool enableDebug);
	// string SetRequestTimeout(int timeout);
	// string SetDefaultValue(string parameterKey, string defaultValue);
	// string GetMe();
	// string StartGetMe();
	// string GetUpdates();
	// string StartGetUpdates();
	// string SendDocument(string chatId, string file);
	// string StartSendDocument(string chatId, string file);
	// string SendPhoto(string chatId, string photoFile);
	// string StartSendPhoto(string chatId, string photoFile);
	// string SendText(string chatId, string chatText);
	// string StartSendText(string chatId, string chatText);
	// string GetMessageByCorrelationId(string correlationKey);


//+------------------------------------------------------------------+
//| IMPORTANT														 |
//| Before this code is able to run you may have to switch some dummy|
//| values in the following examples.                                |
//+------------------------------------------------------------------+

//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- create timer
   EventSetTimer(60);

	string resultOfInitialize = TelegramModule::Initialize("876708006:BAFEUxGUwPeLFKwPFu4GWjq0saUmVEsKxb4", 3);
	Sleep(1000);
	string resultOfSetDebugOutput = TelegramModule::SetDebugOutput(true);
	Sleep(1000);
	string resultOfSetRequestTimeout = TelegramModule::SetRequestTimeout(3);
	Sleep(1000);
	string resultOfSetDefaultValue = TelegramModule::SetDefaultValue("ParseMode", "html");
	Sleep(1000);
	string resultOfGetMe = TelegramModule::GetMe();
	Sleep(1000);
	string resultOfStartGetMe = TelegramModule::StartGetMe();
	Sleep(1000);
	string resultOfGetUpdates = TelegramModule::GetUpdates();
	Sleep(1000);
	string resultOfStartGetUpdates = TelegramModule::StartGetUpdates();
	Sleep(1000);
	string resultOfSendDocument = TelegramModule::SendDocument("-1001167825793", "D:/pathToFile/log.txt");
	Sleep(1000);
	string resultOfStartSendDocument = TelegramModule::StartSendDocument("-1001167825793", "D:/pathToFile/log.txt");
	Sleep(1000);
	string resultOfSendPhoto = TelegramModule::SendPhoto("-1001167825793", "D:/pathToPhoto/photo.png");
	Sleep(1000);
	string resultOfStartSendPhoto = TelegramModule::StartSendPhoto("-1001167825793", "D:/pathToPhoto/photo.png");
	Sleep(1000);
	string resultOfSendText = TelegramModule::SendText("-1001167825793", "Some text");
	Sleep(1000);
	string resultOfStartSendText = TelegramModule::StartSendText("-1001167825793", "Some text in <b>bold</b> with smile emoji: \U+1F601");
	Sleep(1000);
	string resultOfGetMessageByCorrelationId = TelegramModule::GetMessageByCorrelationId("w8er4345grt76567");
	Sleep(1000);


   return(INIT_SUCCEEDED);
  }
//+------------------------------------------------------------------+
//| Expert deinitialization function                                 |
//+------------------------------------------------------------------+
void OnDeinit(const int reason)
  {
//--- destroy timer
   EventKillTimer();
   
  }
//+------------------------------------------------------------------+
//| Expert tick function                                             |
//+------------------------------------------------------------------+
void OnTick()
  {
//---
   
  }
//+------------------------------------------------------------------+
//| Timer function                                                   |
//+------------------------------------------------------------------+
void OnTimer()
  {
//---
   
  }
//+------------------------------------------------------------------+
//| Trade function                                                   |
//+------------------------------------------------------------------+
void OnTrade()
  {
//---
   
  }
//+------------------------------------------------------------------+
//| TradeTransaction function                                        |
//+------------------------------------------------------------------+
void OnTradeTransaction(const MqlTradeTransaction& trans,
                        const MqlTradeRequest& request,
                        const MqlTradeResult& result)
  {
//---
   
  }
//+------------------------------------------------------------------+
//| Tester function                                                  |
//+------------------------------------------------------------------+
double OnTester()
  {
//---
   double ret=0.0;
//---

//---
   return(ret);
  }
//+------------------------------------------------------------------+
//| TesterInit function                                              |
//+------------------------------------------------------------------+
void OnTesterInit()
  {
//---
   
  }
//+------------------------------------------------------------------+
//| TesterPass function                                              |
//+------------------------------------------------------------------+
void OnTesterPass()
  {
//---
   
  }
//+------------------------------------------------------------------+
//| TesterDeinit function                                            |
//+------------------------------------------------------------------+
void OnTesterDeinit()
  {
//---
   
  }
//+------------------------------------------------------------------+
//| ChartEvent function                                              |
//+------------------------------------------------------------------+
void OnChartEvent(const int id,
                  const long &lparam,
                  const double &dparam,
                  const string &sparam)
  {
//---
   
  }
//+------------------------------------------------------------------+
//| BookEvent function                                               |
//+------------------------------------------------------------------+
void OnBookEvent(const string &symbol)
  {
//---
   
  }
//+------------------------------------------------------------------+
