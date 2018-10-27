const path = require("path");
const webpack = require("webpack");
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');

function resolve(filePath) {
    return path.join(__dirname, filePath)
}

var isProduction = !process.argv.find(v => v.indexOf('webpack-dev-server') !== -1);
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");
var commonPlugins = [
    new HtmlWebpackPlugin({
        filename: 'index.html',
        template: resolve('index.html')
    })
];

module.exports = {
    mode: isProduction ? "production" : "development",
    entry:    ["whatwg-fetch","@babel/polyfill", resolve("TypingSpeed.fsproj")],
    output: {
        path: resolve("../Server/wwwroot"),
        filename:  '[name].js',
        devtoolModuleFilenameTemplate: info =>
            path.resolve(info.absoluteResourcePath).replace(/\\/g, '/'),
    },
    plugins: isProduction ?
        commonPlugins.concat([
            new CopyWebpackPlugin([
                { from: resolve('/static') }
            ])
        ])
        : commonPlugins.concat([
            new webpack.HotModuleReplacementPlugin(),
            new webpack.NamedModulesPlugin()
        ]),
    devtool: isProduction ? 'undefined':'source-map',
    devServer: {
        contentBase: resolve("/static"),
        port: 8081,
        hot: true,
        proxy: {'/api': 'http://localhost:5000'},
        historyApiFallback: true
    },
    module: {
        rules: [{
            test: /\.fs(x|proj)?$/,
            use: {
                loader: "fable-loader",
            }
        },
        {
            test: /\.js$/,
            exclude: /node_modules/,
            use: {
              loader: 'babel-loader',
              options:  {
                presets: [
                    ["@babel/preset-env", {
        
                        "targets": {
                        },
                        "modules": false,
                        "useBuiltIns": "usage",
                    }]
                ],
                plugins: ["@babel/plugin-transform-runtime"]
                }
            }
        }
    
    ]
    }
}
