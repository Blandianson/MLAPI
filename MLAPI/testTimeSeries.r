###AUthor: Nicole Jackson (nicole.jackson@halobi.com)
###Date: 10 January 2019
###Description: Takes Time Series data and returns a forcast - for Halo Wep App and MLWorkbench.

#!/usr/bin/env Rscript


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
  
  for (i in args[4]){
    rows <- unlist(strsplit(i, "\r\n"))                         #Splits TS into rows
    rows <- c(rows, str(i))                                     #its adding only a space for every argument
  }

  
  for (line in rows){
    x <- unlist(strsplit(line, ";"))                            #Splits TS rows into indiv. elements
    levelVector <- append(levelVector, as.numeric(x[as.numeric(column)]))   #collects only the data (x[2]), rather than the date (x[1])
  }
  cat(startYear, startMonth, endYear, endMonth)
  
  
  levelTs <- ts(levelVector, start=c(startYear, startMonth), end=c(endYear, endMonth), frequency=12)
  #plot(levelTs)
  
  require("forecast")
  library(forecast)
  findbest <- auto.arima(levelTs)
  #plot(forecast(findbest, h=20))
  write.csv(forecast(findbest, points), "forecastOut.csv", row.names = FALSE)
  
}