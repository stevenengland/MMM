#property copyright "Copyright 2019, StEn"
#property link      "https://steven-england.info"
#property version   "1.1"

#import "Mql_Telegram.dll"
	string Initialize(string apiKey, int timeout);
	string SetDebugOutput(bool enableDebug);
	string SetRequestTimeout(int timeout);
	string GetMe();
	string StartGetMe();
	string SendText(string chatId, string chatText);
	string StartSendText(string chatId, string chatText);
	string GetMessageByCorrelationId(string correlationKey);
#import

//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- create timer
   EventSetTimer(60);

	string resultOfInitialize = Initialize("876708006:BAFEUxGUwPeLFKwPFu4GWjq0saUmVEsKxb4", 3);
	Sleep(1000);
	string resultOfSetDebugOutput = SetDebugOutput(true);
	Sleep(1000);
	string resultOfSetRequestTimeout = SetRequestTimeout(3);
	Sleep(1000);
	string resultOfGetMe = GetMe();
	Sleep(1000);
	string resultOfStartGetMe = StartGetMe();
	Sleep(1000);
	string resultOfSendText = SendText("-1001167825793", "Some text");
	Sleep(1000);
	string resultOfStartSendText = StartSendText("-1001167825793", "Some text");
	Sleep(1000);
	string resultOfGetMessageByCorrelationId = GetMessageByCorrelationId("w8er4345grt76567");
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
