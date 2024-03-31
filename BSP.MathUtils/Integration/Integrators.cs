namespace BSP.MathUtils.Integration;

public class Integrators
{
    public static double TrapezoidMethod(Func<double, double> func, double start, double end, int N, CancellationToken token)
    {
        var step = (end - start) / N;
        var sum = 0.5 * (func(start) + func(end));
        for (var i = 1; i < N - 1 && !token.IsCancellationRequested; i++)
        {
            sum += func(start + i * step);
        }
        return !token.IsCancellationRequested ? sum * step : 0;
    }

    public static double SimpsonMethod(Func<double, double> func, double start, double end, int N, CancellationToken token)
    {
        if (N % 2 != 0)
            N += 1;

        var step = (end - start) / N;
        var offset = step;
        var m = 4;
        var sum = func(start) + func(end);
        for (var i = 0; i < N - 1 && !token.IsCancellationRequested; i++)
        {
            sum += m * func(start + offset);
            m = 6 - m;
            offset += step;
        }
        return !token.IsCancellationRequested ? sum * step / 3.0 : 0;
    }

    public static double Integrate(Func<double, double> func, double start, double end, int N, CancellationToken token)
    {
        return SimpsonMethod(func, start, end, N, token);
    }

    public static double Integrate(Func<double, double, double> func, double startA, double endA, int NA, double startB, double endB, int NB, CancellationToken token)
    {
        return Integrate(
            y => Integrate(
                x => func(x, y),
                startA, endA, NA, token),
            startB, endB, NB, token);
    }


    public static double Integrate(Func<double, double, double, double> func, double startA, double endA, int NA, double startB, double endB, int NB, double startC, double endC, int NC, CancellationToken token)
    {
        return Integrate(
            z => Integrate(
                y => Integrate(
                    x => func(x, y, z),
                    startA, endA, NA, token),
                startB, endB, NB, token),
            startC, endC, NC, token);
    }
}