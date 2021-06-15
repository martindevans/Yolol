using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yolol.Execution;

namespace YololEmulator.Tests.Ladder
{
    /*
        if input_countdown==100 then ispeed=input_speed speed=ispeed end
        if input_countdown==100 then idist=input_dist dist=idist end
        if :t<-1 then :fail="Throttle cannot be < -1" goto11 end
        if :t>1 then :fail="Throttle cannot be > 1" goto11 end
        speed+=:t dist-=speed :speed=speed :dist=dist
        if dist<0 then goto10 end
        goto:done++


        :fail="You crashed after " + ((100-input_countdown)+1) + " steps!"
        :fail+=" Initial Speed: " + ispeed
        :fail+=" Final Speed: " + speed
        :fail+=" Initial Distance: " + idist
        :fail+=" Final Distance: " + dist
        :done=1
     */

    [TestClass]
    public class InteractiveDocking
    {
        [TestMethod]
        public void GenerateInteractiveDocking()
        {
            var input = new List<Dictionary<string, Value>>();
            var output = new List<Dictionary<string, Value>>();

            var rng = new Random(6573);

            for (var challenge = 0; challenge < 100; challenge++)
            {
                for (var i = 100; i >= 0; i--)
                {
                    var din = new Dictionary<string, Value> {{"countdown", (Value)i}};
                    if (i == 100)
                    {
                        // Set initial speed and distance
                        var speed = 10 * rng.NextDouble();
                        var dist = 50 + speed * rng.NextDouble() * 10;
                        din["speed"] = (Number)speed;
                        din["dist"] = (Number)dist;
                    }
                    input.Add(din);

                    // always target exactly zero distance from station
                    var dout = new Dictionary<string, Value> {{"dist", (Value)0}};
                    output.Add(dout);
                }
            }

            Generator.YololLadderGenerator(input, output, false, Generator.ScoreMode.Approximate);
        }
    }
}