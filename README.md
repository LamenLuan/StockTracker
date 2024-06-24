# Stock Tracker
This project aims to assist Brazilian investors by notifying them when a target price, predefined by the user, for a stock in the Brazilian market is reached or is close to being reached.

The project contains two programs that run locally. [StockTracker](StockTracker) is a Windows application that runs in the background, tracking the prices of stocks defined by the user in the [StockTrackerConfigurator](StockTrackerConfigurator) web application.

Currently, stock prices are retrieved using the free API [brapi](https://brapi.dev), and notifications are delivered via Windows toasts. The next steps are to add optional email notifications and create our own API to read stock prices.
