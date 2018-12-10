# Application Insights Demo
Demonstration of several aspects of Application Insights:

- [Data Filtering](https://github.com/Ibis-Software/AppInsightsDemo/blob/master/src/AppInsightDemo/AppInsights/CustomTelemetryFilter.cs)
- [Data Enrichment](https://github.com/Ibis-Software/AppInsightsDemo/blob/master/src/AppInsightDemo/AppInsights/CustomInitializer.cs)
- Using [Custom Tracking](https://github.com/Ibis-Software/AppInsightsDemo/blob/master/src/AppInsightDemo/AppInsights/DurationTracker.cs)
- [Configuration](https://github.com/Ibis-Software/AppInsightsDemo/blob/master/src/AppInsightDemo/Startup.cs)

# Deploy

This ARM template will create a website with an Application Insights resource attached to it.

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FIbis-software%2FAppInsightsDemo%2Fmaster%2Fsrc%2FAppInsightDemoResources%2FWebSite.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>
