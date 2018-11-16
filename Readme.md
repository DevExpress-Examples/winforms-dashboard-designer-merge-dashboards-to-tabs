# How to combine multiple dashboards into a dashboard with multiple tab pages

This example creates a single merged dashboard from multiple individual dashboards, inserting them as tab pages in a tab container.

To insert a dashboard as a tab page contained in another dashboard, the application should perform the following actions in code:
* Load the specified dashboard and check whether it has a tab container. 
* Copy dashboard data sources and resolve name conflicts. Create a dictionary containing old and new names.
* Copy dashboard groups and resolve name conflicts. Create a dictionary containing old and new names.
* Copy dashboard items except groups and resolve name conflicts. Create a dictionary containing old and new names. Update data bindings using the data source names dictionary created in Step 1.
* Update the layout. Move the layout root to the tab page as a child node. Item and group names specified as source items for the layout items should be updated if they are changed while resolving name conflicts.


![](https://github.com/DevExpress-Examples/winforms-dashboard-designer-merge-dashboards-to-tabs/blob/18.2.3%2B/images/Screenshot.png)

In a merged dashboard, you can clean up the data source collection by discovering identical data sources with different names and using the [Data Source Browser](http://newdoc.devexpress.devx/Dashboard/15611/building-the-designer-and-viewer-applications/winforms-designer/ui-elements/data-source-browser) command button to switch the data source for the selected dashboard item:

![](~/images/DataSourceBrowser.png)

