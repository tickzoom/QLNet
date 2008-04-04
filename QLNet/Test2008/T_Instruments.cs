﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QLNet;

namespace TestSuite {
    public class InstrumentTest {
        void testObservable() {
            Console.WriteLine("Testing observability of instruments...");

            SimpleQuote me1 = new SimpleQuote(0.0);
            RelinkableHandle<Quote> h = new RelinkableHandle<Quote>(me1);
            Instrument s = new Stock(h);

            Flag f = new Flag();

            s.registerWith(f.update);
            
            s.NPV();
            me1.setValue(3.14);
            if (!f.isUp())
                Console.WriteLine("Observer was not notified of instrument change");
            
            s.NPV();
            f.lower();
            SimpleQuote me2 = new SimpleQuote(0.0);
            h.linkTo(me2);
            if (!f.isUp())
                Console.WriteLine("Observer was not notified of instrument change");

            f.lower();
            s.freeze();
            s.NPV();
            me2.setValue(2.71);
            if (f.isUp())
                Console.WriteLine("Observer was notified of frozen instrument change");
            s.NPV();
            s.unfreeze();
            if (!f.isUp())
                Console.WriteLine("Observer was not notified of instrument change");
        }

        public void suite() {
            testObservable();
        }
    }
}
