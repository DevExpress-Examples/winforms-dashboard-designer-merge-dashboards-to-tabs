# How to Combine Multiple Dashboards into a Dashboard with Multiple Tab Pages

This example creates a single merged dashboard from multiple individual dashboards, inserting them as tab pages in a tab container.

Run the application. Click the **Merge Dashboard** button in the dashboard title. In the **Open** file dialog select the "sample-dashboard.xml" and "another-sample-dashboard.xml" files (you can select any other dashboard xml definition files) and click **Open**.

![screenshot](/images/Screenshot.png)

To insert a dashboard as a tab page contained in another dashboard, the application performs the following tasks:

1. Load the specified dashboard and check whether it has a tab container. 
2. Copy dashboard data sources and resolve name conflicts. Create a dictionary containing old and new names.
3. Copy dashboard groups and resolve name conflicts. Create a dictionary containing old and new names.
4. Copy dashboard items except groups and resolve name conflicts. Create a dictionary containing old and new names. Update data bindings using the data source names dictionary created while resolving the data source name conflicts.
5. Update the layout. Move the layout root to the tab page as a child node. Item and group names specified as source items for the layout items should be updated if they are changed while resolving name conflicts.


In a merged dashboard, you can clean up the data source collection by discovering identical data sources with different names and using the [Data Source Browser](http://newdoc.devexpress.devx/Dashboard/15611/building-the-designer-and-viewer-applications/winforms-designer/ui-elements/data-source-browser) command button to switch the data source for the selected dashboard item:

![DataSourceBrowser](/images/DataSourceBrowser.png)

