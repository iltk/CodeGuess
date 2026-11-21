namespace CodeGuess.Api.Services;

public static class ScoreCalculator
{
    // formula taken from Kahoot
    //Points=( 1−  Response Time /Total Time for Question  ×0.5)× Max Points
    //response  time: time that took player to answer
    public static double Calculate(double responseTime)
    {
        if (responseTime == 0)
        {
            throw new InvalidOperationException("Time must be greater than zero.");
        }

        //max points that player can get per question
        double maxPoints = 1000;
        double responseRatio = responseTime / 10;

        //award player full points
        if (responseTime <= 0.5)
        {
            responseRatio = 0;
        }

        return Math.Round((1 - (responseRatio * 0.5)) * maxPoints);
    }
}
