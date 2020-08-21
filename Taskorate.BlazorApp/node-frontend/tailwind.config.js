module.exports = {
  purge: {
    enabled: true,
    content: [
      '../wwwroot/index.html',
      '../Pages/**/*.razor',
      '../Shared/**/*.razor',
    ]
  },
  theme: {
    extend: {},
  },
  variants: {
    opacity: ['responsive', 'hover', 'focus', 'group-hover']
  },
  plugins: [],
}
