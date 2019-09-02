//+------------------------------------------------------------------+
//|                                                 JsonExamples.mq5 |
//|                        Copyright 2019, MetaQuotes Software Corp. |
//|                                             https://www.mql5.com |
//+------------------------------------------------------------------+
#property copyright "Copyright 2019, MetaQuotes Software Corp."
#property link      "https://www.mql5.com"
#property version   "1.00"

#include <MMM\JAson.mqh>

//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- create timer
   EventSetTimer(60);
   
   CJAVal jsonLib(NULL,jtUNDEF);
   
   // Deserializing an error structure
   string response = "{\"isSuccess\":false,\"content\":{\"message\":\"The operation was cancelled.\",\"exceptionMessage\":\"The operation was canceled.\",\"exceptionType\":\"OperationCanceledException\",\"stackTrace\":\"   at System.Threading.CancellationToken...\"}}";
   bool isSerializable = jsonLib.Deserialize(response);
   bool isSuccess = jsonLib["isSuccess"].ToBool();
   Print("An error occured: " + jsonLib["content"]["message"].ToStr());
   
   // Deserializing an success structure
   jsonLib.Clear();
   response = "{\"isSuccess\":true,\"content\":{\"id\":876708006,\"is_bot\":true,\"first_name\":\"Test_Mql_Telegram\",\"username\":\"Test_Mql_Telegram_bot\"}}";
   isSerializable = jsonLib.Deserialize(response);
   isSuccess = jsonLib["isSuccess"].ToBool();
   Print("My bot name is: " + jsonLib["content"]["first_name"].ToStr());
   
//---
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
