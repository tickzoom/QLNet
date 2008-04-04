using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QLNet;

namespace TestSuite {
    public class Flag : IObserver {
        private bool up_;

        public Flag() {
            up_ = false;
        }

        public void raise() { up_ = true; }
        public void lower() { up_ = false; }
        public bool isUp() { return up_; }
        public void update() { raise(); }
    };

    public static class Utilities {
        public static YieldTermStructure flatRate(Date today, double forward, DayCounter dc) {
            return new FlatForward(today, new SimpleQuote(forward), dc);
        }
        public static YieldTermStructure flatRate(Date today, Quote forward, DayCounter dc) {
            return new FlatForward(today, forward, dc);
        }
    }
}
