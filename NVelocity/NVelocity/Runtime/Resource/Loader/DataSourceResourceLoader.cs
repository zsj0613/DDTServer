/*
* Licensed to the Apache Software Foundation (ASF) under one
* or more contributor license agreements.  See the NOTICE file
* distributed with this work for additional information
* regarding copyright ownership.  The ASF licenses this file
* to you under the Apache License, Version 2.0 (the
* "License"); you may not use this file except in compliance
* with the License.  You may obtain a copy of the License at
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing,
* software distributed under the License is distributed on an
* "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
* KIND, either express or implied.  See the License for the
* specific language governing permissions and limitations
* under the License.    
*/

namespace NVelocity.Runtime.Resource.Loader
{
    using System.Data;
    using System.Data.SqlClient;

    using Commons.Collections;
    using Exception;
    using NVelocity.Util;

    /// <summary> <P>This is a simple template file loader that loads templates
    /// from a DataSource instead of plain files.
    /// 
    /// <P>It can be configured with a datasource name, a table name,
    /// id column (name), content column (the template body) and a
    /// datetime column (for last modification info).
    /// <br>
    /// <br>
    /// Example configuration snippet for velocity.properties:
    /// <br>
    /// <br>
    /// resource.loader = file, ds <br>
    /// <br>
    /// ds.resource.loader.public.name = DataSource <br>
    /// ds.resource.loader.description = Velocity DataSource Resource Loader <br>
    /// ds.resource.loader.class = org.apache.velocity.runtime.resource.loader.DataSourceResourceLoader <br>
    /// ds.resource.loader.resource.datasource = java:comp/env/jdbc/Velocity <br>
    /// ds.resource.loader.resource.table = tb_velocity_template <br>
    /// ds.resource.loader.resource.keycolumn = id_template <br>
    /// ds.resource.loader.resource.templatecolumn = template_definition <br>
    /// ds.resource.loader.resource.timestampcolumn = template_timestamp <br>
    /// ds.resource.loader.cache = false <br>
    /// ds.resource.loader.modificationCheckInterval = 60 <br>
    /// <br>
    /// <P>Optionally, the developer can instantiate the DataSourceResourceLoader and set the DataSource via code in
    /// a manner similar to the following:
    /// <BR>
    /// <BR>
    /// DataSourceResourceLoader ds = new DataSourceResourceLoader();<BR>
    /// ds.setDataSource(DATASOURCE);<BR>
    /// Velocity.setProperty("ds.resource.loader.instance",ds);<BR>
    /// <P> The property <code>ds.resource.loader.class</code> should be left out, otherwise all the other
    /// properties in velocity.properties would remain the same.
    /// <BR>
    /// <BR>
    /// 
    /// Example WEB-INF/web.xml: <br>
    /// <br>
    /// <resource-ref> <br>
    /// <description>Velocity template DataSource</description> <br>
    /// <res-ref-name>jdbc/Velocity</res-ref-name> <br>
    /// <res-type>javax.sql.DataSource</res-type> <br>
    /// <res-auth>Container</res-auth> <br>
    /// </resource-ref> <br>
    /// <br>
    /// <br>
    /// and Tomcat 4 server.xml file: <br>
    /// [...] <br>
    /// <Context path="/exampleVelocity" docBase="exampleVelocity" debug="0"> <br>
    /// [...] <br>
    /// <ResourceParams name="jdbc/Velocity"> <br>
    /// <parameter> <br>
    /// <name>driverClassName</name> <br>
    /// <value>org.hsql.jdbcDriver</value> <br>
    /// </parameter> <br>
    /// <parameter> <br>
    /// <name>driverName</name> <br>
    /// <value>jdbc:HypersonicSQL:database</value> <br>
    /// </parameter> <br>
    /// <parameter> <br>
    /// <name>user</name> <br>
    /// <value>database_username</value> <br>
    /// </parameter> <br>
    /// <parameter> <br>
    /// <name>password</name> <br>
    /// <value>database_password</value> <br>
    /// </parameter> <br>
    /// </ResourceParams> <br>
    /// [...] <br>
    /// </Context> <br>
    /// [...] <br>
    /// <br>
    /// Example sql script:<br>
    /// CREATE TABLE tb_velocity_template ( <br>
    /// id_template varchar (40) NOT NULL , <br>
    /// template_definition text (16) NOT NULL , <br>
    /// template_timestamp datetime NOT NULL  <br>
    /// ) <br>
    /// 
    /// </summary>
    /// <author>  <a href="mailto:wglass@forio.com">Will Glass-Husain</a>
    /// </author>
    /// <author>  <a href="mailto:matt@raibledesigns.com">Matt Raible</a>
    /// </author>
    /// <author>  <a href="mailto:david.kinnvall@alertir.com">David Kinnvall</a>
    /// </author>
    /// <author>  <a href="mailto:paulo.gaspar@krankikom.de">Paulo Gaspar</a>
    /// </author>
    /// <author>  <a href="mailto:lachiewicz@plusnet.pl">Sylwester Lachiewicz</a>
    /// </author>
    /// <author>  <a href="mailto:henning@apache.org">Henning P. Schmiedehausen</a>
    /// </author>
    /// <version>  $Id: DataSourceResourceLoader.java 687177 2008-08-19 22:00:32Z nbubna $
    /// </version>
    /// <since> 1.5
    /// </since>
    public class DataSourceResourceLoader : ResourceLoader
    {
        /// <summary> Set the DataSource used by this resource loader.  Call this as an alternative to
        /// specifying the data source name via properties.
        /// </summary>
        /// <param name="dataSource">The data source for this ResourceLoader.
        /// </param>
        public virtual string DataSource
        {
            set
            {
                this.dataSource = value;
            }

        }
        private System.String dataSourceName;
        private System.String tableName;
        private System.String keyColumn;
        private System.String templateColumn;
        private System.String timestampColumn;
        private string dataSource;

        /// <seealso cref="org.apache.velocity.runtime.resource.loader.ResourceLoader.init(org.apache.commons.collections.ExtendedProperties)">
        /// </seealso>
        public override void Init(ExtendedProperties configuration)
        {
            dataSourceName = StringUtils.NullTrim(configuration.GetString("resource.datasource"));
            tableName = StringUtils.NullTrim(configuration.GetString("resource.table"));
            keyColumn = StringUtils.NullTrim(configuration.GetString("resource.keycolumn"));
            templateColumn = StringUtils.NullTrim(configuration.GetString("resource.templatecolumn"));
            timestampColumn = StringUtils.NullTrim(configuration.GetString("resource.timestampcolumn"));

            if (dataSource != null)
            {
                if (log.DebugEnabled)
                {
                    log.Debug("DataSourceResourceLoader: using dataSource instance with table \"" + tableName + "\"");
                    log.Debug("DataSourceResourceLoader: using columns \"" + keyColumn + "\", \"" + templateColumn + "\" and \"" + timestampColumn + "\"");
                }

                log.Trace("DataSourceResourceLoader initialized.");
            }
            else if (dataSourceName != null)
            {
                if (log.DebugEnabled)
                {
                    log.Debug("DataSourceResourceLoader: using \"" + dataSourceName + "\" datasource with table \"" + tableName + "\"");
                    log.Debug("DataSourceResourceLoader: using columns \"" + keyColumn + "\", \"" + templateColumn + "\" and \"" + timestampColumn + "\"");
                }

                log.Trace("DataSourceResourceLoader initialized.");
            }
            else
            {
                System.String msg = "DataSourceResourceLoader not properly initialized. No DataSource was identified.";
                log.Error(msg);
                throw new System.SystemException(msg);
            }
        }

        /// <seealso cref="org.apache.velocity.runtime.resource.loader.ResourceLoader.isSourceModified(org.apache.velocity.runtime.resource.Resource)">
        /// </seealso>
        public override bool IsSourceModified(Resource resource)
        {
            return (resource.LastModified != ReadLastModified(resource, "checking timestamp"));
        }

        /// <seealso cref="org.apache.velocity.runtime.resource.loader.ResourceLoader.getLastModified(org.apache.velocity.runtime.resource.Resource)">
        /// </seealso>
        public override long GetLastModified(Resource resource)
        {
            return ReadLastModified(resource, "getting timestamp");
        }

        /// <summary> Get an InputStream so that the Runtime can build a
        /// template with it.
        /// 
        /// </summary>
        /// <param name="name">name of template
        /// </param>
        /// <returns> InputStream containing template
        /// </returns>
        /// <throws>  ResourceNotFoundException </throws>
        public override System.IO.Stream GetResourceStream(System.String name)
        {
            lock (this)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ResourceNotFoundException("DataSourceResourceLoader: Template name was empty or null");
                }
                IDbConnection conn = null;
                IDataReader rs = null;

                try
                {
                    conn = OpenDbConnection();
                    rs = ReadData(conn, templateColumn, name);

                    if (rs.Read())
                    {
                        System.IO.Stream stream = new System.IO.MemoryStream((byte[])rs[templateColumn]);
                        if (stream == null)
                        {
                            throw new ResourceNotFoundException("DataSourceResourceLoader: " + "template column for '" + name + "' is null");
                        }

                        return new System.IO.BufferedStream(stream);
                    }
                    else
                    {
                        throw new ResourceNotFoundException("DataSourceResourceLoader: " + "could not find resource '" + name + "'");
                    }
                }
                catch (System.Data.OleDb.OleDbException sqle)
                {
                    System.String msg = "DataSourceResourceLoader: database problem while getting resource '" + name + "': ";

                    log.Error(msg, sqle);
                    throw new ResourceNotFoundException(msg);
                }
                catch (System.Exception ne)
                {
                    System.String msg = "DataSourceResourceLoader: database problem while getting resource '" + name + "': ";

                    log.Error(msg, ne);
                    throw new ResourceNotFoundException(msg);
                }
                finally
                {
                    CloseResultSet(rs);
                    CloseDbConnection(conn);
                }
            }
        }

        /// <summary> Fetches the last modification time of the resource
        /// 
        /// </summary>
        /// <param name="resource">Resource object we are finding timestamp of
        /// </param>
        /// <param name="operation">string for logging, indicating caller's intention
        /// 
        /// </param>
        /// <returns> timestamp as long
        /// </returns>
        private long ReadLastModified(Resource resource, System.String operation)
        {
            long timeStamp = 0;

            /* get the template name from the resource */
            System.String name = resource.Name;
            if (name == null || name.Length == 0)
            {
                System.String msg = "DataSourceResourceLoader: Template name was empty or null";
                log.Error(msg);
                throw new System.NullReferenceException(msg);
            }
            else
            {
                IDbConnection conn = null;

                IDataReader rs = null;

                try
                {
                    conn = OpenDbConnection();
                    rs = ReadData(conn, timestampColumn, name);

                    if (rs.Read())
                    {
                        System.DateTime ts = System.Convert.ToDateTime(rs[timestampColumn]);

                        timeStamp = ts != null ? ts.Ticks : 0;
                    }
                    else
                    {
                        System.String msg = "DataSourceResourceLoader: could not find resource " + name + " while " + operation;
                        log.Error(msg);
                        throw new ResourceNotFoundException(msg);
                    }
                }
                catch (System.Data.OleDb.OleDbException sqle)
                {
                    System.String msg = "DataSourceResourceLoader: database problem while " + operation + " of '" + name + "': ";

                    log.Error(msg, sqle);
                    throw new RuntimeException(msg, sqle);
                }
                catch (System.Exception ne)
                {
                    System.String msg = "DataSourceResourceLoader: database problem while " + operation + " of '" + name + "': ";

                    log.Error(msg, ne);
                    throw new RuntimeException(msg, ne);
                }
                finally
                {
                    CloseResultSet(rs);
                    CloseDbConnection(conn);
                }
            }
            return timeStamp;
        }

        /// <summary> Gets connection to the datasource specified through the configuration
        /// parameters.
        /// 
        /// </summary>
        /// <returns> connection
        /// </returns>
        private IDbConnection OpenDbConnection()
        {
            if (!string.IsNullOrEmpty(dataSource))
            {
                IDbConnection temp_Connection = new SqlConnection();
                temp_Connection.Open();
                return temp_Connection;
            }

            return null;
        }

        /// <summary> Closes connection to the datasource</summary>
        private void CloseDbConnection(IDbConnection conn)
        {
            if (conn != null)
            {
                try
                {
                    conn.Close();
                }
                catch (System.SystemException re)
                {
                    throw re;
                }
                catch (System.Exception e)
                {
                    System.String msg = "DataSourceResourceLoader: problem when closing connection";
                    log.Error(msg, e);
                    throw new VelocityException(msg, e);
                }
            }
        }

        /// <summary> Closes the result set.</summary>
        private void CloseResultSet(IDataReader rs)
        {
            if (rs != null)
            {
                try
                {
                    rs.Close();
                }
                catch (System.SystemException re)
                {
                    throw re;
                }
                catch (System.Exception e)
                {
                    System.String msg = "DataSourceResourceLoader: problem when closing result set";
                    log.Error(msg, e);
                    throw new VelocityException(msg, e);
                }
            }
        }

        /// <summary> Reads the data from the datasource.  It simply does the following query :
        /// <br>
        /// SELECT <i>columnNames</i> FROM <i>tableName</i> WHERE <i>keyColumn</i>
        /// = '<i>templateName</i>'
        /// <br>
        /// where <i>keyColumn</i> is a class member set in init()
        /// 
        /// </summary>
        /// <param name="conn">connection to datasource
        /// </param>
        /// <param name="columnNames">columns to fetch from datasource
        /// </param>
        /// <param name="templateName">name of template to fetch
        /// </param>
        /// <returns> result set from query
        /// </returns>
        private IDataReader ReadData(IDbConnection conn, System.String columnNames, System.String templateName)
        {
            IDbCommand ps = conn.CreateCommand();
            ps.CommandText = "SELECT " + columnNames + " FROM " + tableName + " WHERE " + keyColumn + " = '" + templateName + "'";

            return ps.ExecuteReader();
        }
    }
}
