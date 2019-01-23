###AUthor: Nicole Jackson (nicole.jackson@halobi.com)
###Date: 10 January 2019
###Description: Takes Time Series data and returns a forcast - for Halo Wep App and MLWorkbench.

#!/usr/bin/env Rscript

setwd("C:\\Users\\Nicole.jackson\\source\\repos\MLAPI\\Data")
args = commandArgs(trailingOnly = TRUE)

if(length(args) == 0){
    
    stop("Please input time series data.")
    
}else if (length(args) >= 1){
    
    levelVector = c()
    inData = c()
    startYear = as.numeric(substr(args[1], 1, 4))
    startMonth = as.numeric(substr(args[1], 6, 7))
    endYear = as.numeric(substr(args[2], 1, 4))
    endMonth = as.numeric(substr(args[2], 6, 7))
    column = args[3]
    data = read.csv(args[4], header=TRUE, sep=",", dec=".")
    # 
    # levelTs <- ts(data, start=c(startYear, startMonth), end=c(endYear, endMonth), frequency=12)
    # 
    # require("forecast")
    # library(forecast)
    # findbest <- auto.arima(levelTs)
    # #plot(forecast(findbest, h=20))
    # write.csv(forecast(findbest, points), "forecastOut.csv", row.names = FALSLE)
    write.csv(data, file = "C:\\Users\\Nicole.jackson\\source\\repos\\MLAPI\\Data\\forecastOut.csv", row.names = FALSE)
}