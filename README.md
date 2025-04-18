# Stock Tracker
This project aims to assist brazilian investors by notifying them when a target price, predefined by the user, for a stock in the brazilian market is reached or is close to being reached.

The project contains two programs that run locally. [StockTrackerService](StockTrackerService) is a Windows application that runs in the background, tracking the prices of stocks defined by the user in the [StockTracker](StockTracker) web application.

Currently, stock prices are retrieved using the free API [brapi](https://brapi.dev), and notifications are delivered via Windows toasts. The next steps are to add optional email notifications and create our own API to read stock prices.

## Project Requirements
- .NET 8.0
- Windows 10.0.17763.0 or newer

## How to install it

1. Register an account on [brapi](https://brapi.dev) to generate your own API key;

1. Run "StockTracker.exe" to open the web app and inform your API key in it;