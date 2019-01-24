###AUthor: Nicole Jackson (nicole.jackson@halobi.com)
###Date: 10 January 2019
###Description: Takes Time Series data and returns a forcast - for Halo Wep App and MLWorkbench.

#!/usr/bin/env Rscript


#args = commandArgs(trailingOnly = TRUE)

#if(length(args) == 0){
    
    #stop("Please input time series data.")
    
#}else if (length(args) >= 1){
    
    #startYear = args[1]
    #endYear = args[2]
    #column = args[3]
    #data = read.csv(args[4], header=TRUE, sep=",", dec=".")
    #write.csv(data, file = "C:\\Users\\Nicole.jackson\\source\\repos\\MLAPI\\Data\\forecastOut.csv", row.names = FALSE)
	#write.csv(data, file = "C:\\Users\\Nicole.jackson\\source\\repos\\MLAPI\\Data\\outputCsv.csv")
	setwd("C:\\Users\\Nicole.jackson\\source\\repos\MLAPI\\Data")
	sink("mayItWork.txt")
	print("Hope this works!")
	sink()
#}