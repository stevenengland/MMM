#property copyright "Copyright 2019, StEn"
#property link      "https://steven-england.info"
#property version   "1.0"

#import "StEn.MMM.Mql.Mail.dll"
// Available functions:
	// string Initialize(string smtpServer, int smtpPort, string smtpUserName, string smtpPassword, int timeout);
	// string SetDebugOutput(bool enableDebug);
	// string SetRequestTimeout(int timeout);
	// string SetDefaultValue(string parameterKey, string defaultValue);
	// string SendMail(string sender, string &[] recipients, string subject, string textBody, string htmlBody);
	// string StartSendMail(string sender, string &[] recipients, string subject, string textBody, string htmlBody);
	// string SendMail(string sender, string &[] recipients, string subject, string textBody, string htmlBody, string &[] attachments);
	// string StartSendMail(string sender, string &[] recipients, string subject, string textBody, string htmlBody, string &[] attachments);
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

	string resultOfInitialize = MailModule::Initialize("smtp.strato.de", 465, "integration_test@mmm.steven-england.info", "aksddf933++", 10);
	Sleep(1000);
	string resultOfSetDebugOutput = MailModule::SetDebugOutput(true);
	Sleep(1000);
	string resultOfSetRequestTimeout = MailModule::SetRequestTimeout(30);
	Sleep(1000);
	string resultOfSetDefaultValue = MailModule::SetDefaultValue("CheckServerCertificate", "true");
	Sleep(1000);
	// See the different usage and writing possibilities of the recipients:
	// You need one ore more 'to' addresses and optional 'cc', 'bcc'.
	// You can write plain mail addresses or those with a nick name.
	string recipients[] = {"to:recipient1 <r1@sten.info>", "to:r2@sten.info", "cc:r3@sten.info", "bcc:r4@sten.info"};
	string resultOfSendMail = MailModule::SendMail("StEn <test@sten.info>", recipients, "test subject", "plain text body", "<b>html</b> body");
	Sleep(1000);
	string resultOfStartSendMail = MailModule::StartSendMail("StEn <test@sten.info>", recipients, "test subject", "plain text body", "<b>html</b> body");
	Sleep(1000);
	string attachments[1];
	attachments[0] = TerminalInfoString(TERMINAL_DATA_PATH) + "\\MQL5\\Libraries\\"+"photo.png";
	resultOfSendMail = MailModule::SendMail("StEn <test@sten.info>", recipients, "test subject", "plain text body", "<b>html</b> body", attachments);
	Sleep(1000);
	resultOfStartSendMail = MailModule::StartSendMail("StEn <test@sten.info>", recipients, "test subject", "plain text body", "<b>html</b> body", attachments);
	Sleep(1000);
	string resultOfGetMessageByCorrelationId = MailModule::GetMessageByCorrelationId("w8er4345grt76567");
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
