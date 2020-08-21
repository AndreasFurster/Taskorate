const path = require("path");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = {
    module: {
        rules: [
          {
              test: /\.(js|jsx)$/,
              exclude: /node_modules/,
              use: {
                  loader: "babel-loader",
                  options: {
                    presets: ['@babel/preset-env']
                  }
              }
          }, 
          {
            test: /\.(sa|sc|c)ss$/,
            use: [
              {
                loader: MiniCssExtractPlugin.loader
              },
              {
                // This loader resolves url() and @imports inside CSS
                loader: "css-loader",
              },
              {
                // Then we apply postCSS fixes like autoprefixer and minifying
                loader: "postcss-loader"
              },
              {
                // First we transform SASS to standard CSS
                loader: "sass-loader",
                options: {
                  implementation: require("sass")
                }
              }
            ]
          },
        ]
    },
    output: {
        path: path.resolve(__dirname, '../wwwroot/dist'),
        filename: "app.js",
        library: "Taskorate"
    },
    plugins: [
      new MiniCssExtractPlugin({
        filename: "app.css"
      })
    ]
};