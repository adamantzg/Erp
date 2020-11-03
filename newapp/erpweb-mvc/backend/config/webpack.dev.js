const path = require('path');
const rxPaths = require('rxjs/_esm5/path-mapping');

const webpack = require('webpack');

const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const CleanWebpackPlugin = require('clean-webpack-plugin');

const helpers = require('./webpack.helpers');

const ROOT = path.resolve(__dirname, '..');

console.log('@@@@@@@@@ USING DEVELOPMENT @@@@@@@@@@@@@@@');
console.log(__dirname);

module.exports = {

    devtool: 'source-map',
    performance: {
        hints: false
    },
    entry: {
        'polyfills': './Angular5App/polyfills.ts',
        'app': './Angular5App/main.ts'
    },

    output: {
        path: ROOT + '/distdev/',
        filename: '[name].bundle.js',
        chunkFilename: '[id].chunk.js',
        publicPath: '/'
    },

    resolve: {
        extensions: ['.ts', '.js', '.json'],
        alias: rxPaths()
    },

    devServer: {
        historyApiFallback: true,
        contentBase: path.join(ROOT, '/wwwroot/'),
        watchOptions: {
            aggregateTimeout: 300,
            poll: 1000
        }
    },

    module: {
        rules: [
            {
                test: /\.ts$/,
                use: [
                    'awesome-typescript-loader',                    
                    'angular-router-loader',
                    'angular2-template-loader',
                    'source-map-loader'                
                ]
            },
            {
                test: /\.(png|jpg|gif|woff|woff2|ttf|svg|eot)$/,
                use: 'file-loader?name=assets/[name]-[hash:6].[ext]'
            },
            {
                test: /favicon.ico$/,
                use: 'file-loader?name=/[name].[ext]'
            },
            {
                test: /\.css$/,
                use: [
                    'to-string-loader',
                    'css-loader'
                ]
            },
            //{
            //    test: /\.scss$/,
            //    /*include: path.join(ROOT, 'Angular5App/styles'),*/
            //    use: [
            //        'style-loader',
            //        'css-loader',
            //        'sass-loader'
            //    ]
            //},
            //{
            //    test: /\.scss$/,
            //    /*exclude: path.join(ROOT, 'Angular5App/styles'),*/
            //    use: [
            //        'raw-loader',
            //        'sass-loader'
            //    ]
            //},
            {
                test: /\.html$/,
                use: 'raw-loader'
            }
        ],
        exprContextCritical: false
    },
    plugins: [
        function() {
            this.plugin('watch-run',
                function(watching, callback) {
                    console.log('\x1b[33m%s\x1b[0m', `Begin compile at ${(new Date()).toTimeString()}`);
                    callback();
                });
        },
        
        new webpack.optimize.ModuleConcatenationPlugin(),        

        new CleanWebpackPlugin(
            [
                'distdev'
            ],
            { root: ROOT }
        ),

        /*new HtmlWebpackPlugin({
            filename: 'index.html',
            inject: 'body',
            template: 'Angular5App/index.html'
        }),*/

        new CopyWebpackPlugin([
            { from: './Angular5App/assets/*.*', to: 'assets/', flatten: true }
        ])
    ]

};

