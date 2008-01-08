using System;
using System.Collections.Generic;
using System.Text;

namespace QLNet
{
   /// <summary>
   /// Bulgarian lev
   /// The ISO three-letter code is BGL; the numeric code is 100.
   /// It is divided in 100 stotinki.
   /// </summary>
   public class BGLCurrency : Currency 
   {
      public BGLCurrency() 
      {
         _data = new Data("Bulgarian lev", "BGL", 100,"lv", "", 100,new Rounding(),"%1$.2f %3%");
      }

   }

   /// <summary>
   /// Belarussian ruble
   /// The ISO three-letter code is BYR; the numeric code is 974.
   /// It has no subdivisions.
   /// </summary>
   public class BYRCurrency : Currency
   {
      public BYRCurrency()
      {
         _data = new Data("Belarussian ruble", "BYR", 974,"BR", "", 1,new Rounding(),"%2% %1$.0f");
      }

   }

   /// <summary>
   /// Swiss franc
   /// The ISO three-letter code is CHF; the numeric code is 756.
   /// It is divided into 100 cents.
   /// </summary>
   public class CHFCurrency : Currency
   {
      public CHFCurrency()
      {
         _data = new Data("Swiss franc", "CHF", 756,"SwF", "", 100,new Rounding(),"%3% %1$.2f");
      }

   }

   /// <summary>
   /// Cyprus pound
   /// The ISO three-letter code is CYP; the numeric code is 196.
   /// It is divided in 100 cents.
   /// </summary>
   public class CYPCurrency : Currency
   {
      public CYPCurrency()
      {
         _data = new Data("Cyprus pound", "CYP", 196, "\xA3C", "", 100, new Rounding(), "%3% %1$.2f");
      }

   }

   /// <summary>
   /// Czech koruna
   /// The ISO three-letter code is CZK; the numeric code is 203.
   /// It is divided in 100 haleru.
   /// </summary>
   public class CZKCurrency : Currency
   {
      public CZKCurrency()
      {
         _data = new Data("Czech koruna", "CZK", 203,"Kc", "", 100,new Rounding(),"%1$.2f %3%");
      }

   }

   /// <summary>
   /// Danish krone
   /// The ISO three-letter code is DKK; the numeric code is 208.
   /// It is divided in 100 øre.
   /// </summary>
   public class DKKCurrency : Currency
   {
      public DKKCurrency()
      {
         _data = new Data("Danish krone", "DKK", 208,"Dkr", "", 100,new Rounding(),"%3% %1$.2f");
      }

   }

   /// <summary>
   /// Estonian kroon
   /// The ISO three-letter code is EEK; the numeric code is 233.
   /// It is divided in 100 senti.
   /// </summary>
   public class EEKCurrency : Currency
   {
      public EEKCurrency()
      {
         _data = new Data("Estonian kroon", "EEK", 233,"KR", "", 100,new Rounding(),"%1$.2f %2%");
      }

   }

   /// <summary>
   /// European Euro
   /// The ISO three-letter code is EUR; the numeric code is 978.
   /// It is divided into 100 cents.
   /// </summary>
   public class EURCurrency : Currency
   {
      public EURCurrency()
      {
         _data = new Data("European Euro", "EUR", 978,"", "", 100,new ClosestRounding(2),"%2% %1$.2f");
      }

   }

   /// <summary>
   /// British pound sterling
   /// The ISO three-letter code is GBP; the numeric code is 826.
   /// It is divided into 100 pence.
   /// </summary>
   public class GBPCurrency : Currency
   {
      public GBPCurrency()
      {
         _data = new Data("British pound sterling", "GBP", 826,"\xA3", "p", 100,new Rounding(),"%3% %1$.2f");
      }

   }

   /// <summary>
   /// Hungarian forint
   /// The ISO three-letter code is HUF; the numeric code is 348.
   /// It has no subdivisions.
   /// </summary>
   public class HUFCurrency : Currency
   {
      public HUFCurrency()
      {
         _data = new Data("Hungarian forint", "HUF", 348,"Ft", "", 1,new Rounding(),"%1$.0f %3%");
      }

   }

   /// <summary>
   /// Icelandic krona
   /// The ISO three-letter code is ISK; the numeric code is 352.
   /// It is divided in 100 aurar.
   /// </summary>
   public class ISKCurrency : Currency
   {
      public ISKCurrency()
      {
         _data = new Data("Iceland krona", "ISK", 352,"IKr", "", 100,new Rounding(),"%1$.2f %3%");
      }

   }

   /// <summary>
   /// Lithuanian litas
   /// The ISO three-letter code is LTL; the numeric code is 440.
   /// It is divided in 100 centu.
   /// </summary>
   public class LTLCurrency : Currency
   {
      public LTLCurrency()
      {
         _data = new Data("Lithuanian litas", "LTL", 440,"Lt", "", 100,new Rounding(),"%1$.2f %3%");
      }

   }

   /// <summary>
   /// Latvian lat
   /// The ISO three-letter code is LVL; the numeric code is 428.
   /// It is divided in 100 santims.
   /// </summary>
   public class LVLCurrency : Currency
   {
      public LVLCurrency()
      {
         _data = new Data("Latvian lat", "LVL", 428,"Ls", "", 100,new Rounding(),"%3% %1$.2f");
      }

   }

   /// <summary>
   /// Maltese lira
   /// The ISO three-letter code is MTL; the numeric code is 470.
   /// It is divided in 100 cents.
   /// </summary>
   public class MTLCurrency : Currency
   {
      public MTLCurrency()
      {
         _data = new Data("Maltese lira", "MTL", 470,"Lm", "", 100,new Rounding(),"%3% %1$.2f");
      }

   }

   /// Norwegian krone
   /// The ISO three-letter code is NOK; the numeric code is 578.
   /// It is divided in 100 øre.
   /// </summary>
   public class NOKCurrency : Currency
   {
      public NOKCurrency()
      {
         _data = new Data("Norwegian krone", "NOK", 578,"NKr", "", 100,new Rounding(),"%3% %1$.2f");
      }

   }

   /// Polish zloty
   /// The ISO three-letter code is PLN; the numeric code is 985.
   /// It is divided in 100 groszy.
   /// </summary>
   public class PLNCurrency : Currency
   {
      public PLNCurrency()
      {
         _data = new Data("Polish zloty", "PLN", 985,"zl", "", 100,new Rounding(),"%1$.2f %3%");
      }

   }

   /// Romanian leu
   /// The ISO three-letter code was ROL; the numeric code was 642.
   /// It was divided in 100 bani.
   /// </summary>
   public class ROLCurrency : Currency
   {
      public ROLCurrency()
      {
         _data = new Data("Romanian leu", "ROL", 642,"L", "", 100,new Rounding(),"%1$.2f %3%");
      }

   }

   /// Romanian new leu
   /// The ISO three-letter code is RON; the numeric code is 946.
   /// It is divided in 100 bani.
   /// </summary>
   public class RONCurrency : Currency
   {
      public RONCurrency()
      {
         _data = new Data("Romanian new leu","RON", 946,"L", "", 100,new Rounding(),"%1$.2f %3%");
      }

   }

   /// Swedish krona
   /// The ISO three-letter code is SEK; the numeric code is 752.
   /// It is divided in 100 öre.
   /// </summary>
   public class SEKCurrency : Currency
   {
      public SEKCurrency()
      {
         _data = new Data("Swedish krona", "SEK", 752,"kr", "", 100,new Rounding(),"%1$.2f %3%");
      }

   }

   /// Slovenian tolar
   /// The ISO three-letter code is SIT; the numeric code is 705.
   /// It is divided in 100 stotinov.
   /// </summary>
   public class SITCurrency : Currency
   {
      public SITCurrency()
      {
         _data = new Data("Slovenian tolar", "SIT", 705,"SlT", "", 100,new Rounding(),"%1$.2f %3%");
      }

   }

   /// Slovak koruna
   /// The ISO three-letter code is SKK; the numeric code is 703.
   /// It is divided in 100 halierov.
   /// </summary>
   public class SKKCurrency : Currency
   {
      public SKKCurrency()
      {
         _data = new Data("Slovak koruna", "SKK", 703,"Sk", "", 100,new Rounding(),"%1$.2f %3%");
      }

   }

   /// Turkish lira
   /// The ISO three-letter code was TRL; the numeric code was 792.
   /// It was divided in 100 kurus.
   /// Obsoleted by the new Turkish lira since 2005.
   /// </summary>
   public class TRLCurrency : Currency
   {
      public TRLCurrency()
      {
         _data = new Data("Turkish lira", "TRL", 792,"TL", "", 100,new Rounding(),"%1$.0f %3%");
      }

   }

   /// New Turkish lira
   /// The ISO three-letter code is TRY; the numeric code is 949.
   ///  It is divided in 100 new kurus.
   /// </summary>
   public class TRYCurrency : Currency
   {
      public TRYCurrency()
      {
         _data = new Data("New Turkish lira", "TRY", 949,"YTL", "", 100,new Rounding(),"%1$.2f %3%");
      }

   }

   // currencies obsoleted by Euro
   // not done ( is usefull to do ? 
}
